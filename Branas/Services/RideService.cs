using Branas.Models;
using Npgsql;

namespace Branas.Services
{
    public class RideService
    {
        public async Task<string> RequestRideAsync(RequestRideModel requestRideModel)
        {
            if (!await checkIsPassengerAsync(requestRideModel.PassengerId))
            {
                throw new Exception("Account is not passenger");
            }

            if (await checkIfPassengerHasAnotherRide(requestRideModel.PassengerId))
            {
                throw new Exception("Passenger has another active ride");
            }

            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();

                string rideId = Guid.NewGuid().ToString();
                DateTime date = DateTime.Now;

                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"insert into cccat13.ride (ride_id, passenger_id, status, fare, distance, from_lat, from_long, to_lat, to_long, \"date\") values ('{rideId}', '{requestRideModel.PassengerId}', '{RideStatus.Requested}', 12.5, 100, {requestRideModel.FromLat}, {requestRideModel.FromLong}, {requestRideModel.ToLat}, {requestRideModel.ToLong}, '{date}')";
                await cmd.ExecuteNonQueryAsync();

                return rideId;
            }
            finally
            {
                con.Close();
            }
        }

        public async Task AcceptRideAsync(AcceptRideModel acceptRideModel)
        {
            if (!await checkIsDriverAsync(acceptRideModel.DriverId))
            {
                throw new Exception("Account is not driver");
            }

            if (!await checkIsRideRequested(acceptRideModel.RideId))
            {
                throw new Exception("Ride is not requested");
            }

            if (await checkIfDriverHasAnotherRide(acceptRideModel.DriverId))
            {
                throw new Exception("Driver has another active ride");
            }

            RideModel? ride = await GetRide(acceptRideModel.RideId);

            if (ride is not null)
            {
                ride.SetDriver(acceptRideModel.DriverId);
                ride.UpdateStatus(RideStatus.Accepted);

                await UpdateRide(ride);
            }
        }

        private async Task UpdateRide(RideModel ride)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand($"UPDATE cccat13.ride SET driver_id='{ride.DriverId}', status='{ride.Status}' WHERE ride_id = '{ride.RideId}'", con);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<RideModel?> GetRide(string rideId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand($"select * from cccat13.ride where ride_id = '{rideId}'", con);
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                List<RideModel> result = new();

                while (await reader.ReadAsync())
                {
                    result.Add(new((Guid)reader["ride_id"],
                        (Guid)reader["passenger_id"],
                        reader["driver_id"] != DBNull.Value ? (Guid)reader["driver_id"] : null,
                        (string)reader["status"],
                        (decimal)reader["fare"],
                        (decimal)reader["distance"],
                        (decimal)reader["from_lat"],
                        (decimal)reader["to_lat"],
                        (decimal)reader["from_long"],
                        (decimal)reader["to_long"],
                        (DateTime)reader["date"]));
                }

                return result.FirstOrDefault();
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        private async Task<bool> checkIfPassengerHasAnotherRide(string passengerId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"select ride_id from cccat13.ride where passenger_id = '{passengerId}' and status != 'Cancelled' and status != 'Completed'";
                var result = await cmd.ExecuteScalarAsync();

                return result != null;
            }
            finally
            {
                con.Close();
            }
        }

        private async Task<bool> checkIfDriverHasAnotherRide(string driverId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"select ride_id from cccat13.ride where driver_id = '{driverId}' and status = 'Accepted' or status = 'InProgress'";
                var result = await cmd.ExecuteScalarAsync();

                return result != null;
            }
            finally
            {
                con.Close();
            }
        }

        private async Task<bool> checkIsPassengerAsync(string passengerId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"select is_passenger from cccat13.account where account_id = '{passengerId}'";
                var result = await cmd.ExecuteScalarAsync();

                return result != null && DBNull.Value != result && (bool)result;
            }
            finally
            {
                con.Close();
            }
        }

        private async Task<bool> checkIsDriverAsync(string driverId)
        {
            using NpgsqlConnection con = new("Host=localhost:5432;Username=postgres;Password=01091355@@;Database=app");
            try
            {
                con.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"select is_driver from cccat13.account where account_id = '{driverId}'";
                var result = await cmd.ExecuteScalarAsync();

                return result != null && DBNull.Value != result && (bool)result;
            }
            finally
            {
                con.Close();
            }
        }

        private async Task<bool> checkIsRideRequested(string rideId)
        {
            RideModel? ride = await GetRide(rideId);

            if (ride is null || ride.Status != "Requested")
                return false;

            return true;
        }
    }
}
