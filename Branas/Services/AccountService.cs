using Branas.Models;
using Npgsql;
using System.Text.RegularExpressions;

namespace Branas.Services
{
    public class AccountService
    {
        public CpfValidator CpfValidator { get; set; }

        public AccountService()
        {
            CpfValidator = new CpfValidator();
        }

        public async Task SendEmail(string email, string subject, string message)
        {
            Console.WriteLine(email, subject, message);
        }

        public async Task<string> SignupAsync(SignupModel signup)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                string accountId = Guid.NewGuid().ToString();
                string verificationCode = Guid.NewGuid().ToString();
                var date = new DateTime();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"select * from cccat13.account where email = '{signup.Email}'";
                bool existingAccount = await cmd.ExecuteScalarAsync() != null;

                if (existingAccount)
                {
                    throw new Exception("Account already exists");
                }

                if (!Regex.Match(signup.Name, "[a-zA-Z] [a-zA-Z]+").Success)
                {
                    throw new Exception("Invalid name");
                }

                if (!Regex.Match(signup.Email, "^(.+)@(.+)$").Success)
                {
                    throw new Exception("Invalid email");
                }

                if (!CpfValidator.Validate(signup.CPF))
                {
                    throw new Exception("Invalid cpf");
                }

                if (signup.IsDriver && !Regex.Match(signup.CarPlate, "[A-Z]{3}[0-9]{4}").Success)
                {
                    throw new Exception("Invalid plate");
                }

                cmd.CommandText = $"insert into cccat13.account (account_id, name, email, cpf, car_plate, is_passenger, is_driver, \"date\", is_verified, verification_code) values ('{accountId}', '{signup.Name}', '{signup.Email}', '{signup.CPF}', '{signup.CarPlate}', {signup.IsPassenger}, {signup.IsDriver}, '{date}', {false}, '{verificationCode}')";
                await cmd.ExecuteNonQueryAsync();
                await SendEmail(signup.Email, "Verification", $"Please verify your code at first login ${verificationCode}");

                return accountId;
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<AccountModel?> GetAccount(string accountId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand($"select * from cccat13.account where account_id = '{accountId}'", con);
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                List<AccountModel> result = new();

                while (await reader.ReadAsync())
                {
                    result.Add(new((Guid)reader["account_id"],
                        (string)reader["name"],
                        (string)reader["email"],
                        (string)reader["cpf"],
                        (string)reader["car_plate"],
                        (bool)reader["is_passenger"],
                        (bool)reader["is_driver"],
                        (DateTime)reader["date"],
                        (bool)reader["is_verified"],
                        (Guid)reader["verification_code"]));
                }

                return result.FirstOrDefault();
            }
            finally
            {
                await con.CloseAsync();
            }
        }
    }
}
