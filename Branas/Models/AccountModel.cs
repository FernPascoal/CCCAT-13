namespace Branas.Models
{
    public class AccountModel
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string CarPlate { get; set; }
        public bool IsPassenger { get; set; }
        public bool IsDriver { get; set; }
        public DateTime Date { get; set; }
        public bool IsVerified { get; set; }
        public string VerificationCode { get; set; }

        public AccountModel(Guid accountId, string name, string email, string cpf, string carPlate, bool isPassenger, bool isDriver, DateTime date, bool isVerified, Guid verificationCode)
        {
            AccountId = accountId.ToString();
            Name = name;
            Email = email;
            CPF = cpf;
            CarPlate = carPlate;
            IsPassenger = isPassenger;
            IsDriver = isDriver;
            Date = date;
            IsVerified = isVerified;
            VerificationCode = verificationCode.ToString();
        }
    }
}
