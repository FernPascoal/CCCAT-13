using Branas.Models;
using Branas.Services;

namespace Branas.Test
{
    public class RideServiceTest
    {
        [Fact()]
        public async Task Quando_IsPassengerFalse_NaoDeve_CriarCorrida()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, false, string.Empty);
            AccountService accountService = new();
            string accountId = await accountService.SignupAsync(signup);

            RideService rideService = new();
            RequestRideModel requestRideModel = new(accountId, 12, 40, 10, 29);

            var ex = await Assert.ThrowsAsync<Exception>(() => rideService.RequestRideAsync(requestRideModel));
            Assert.Equal("Account is not passenger", ex.Message);
        }

        [Fact()]
        public async Task Quando_PassageiroTemOutraCorrida_NaoDeve_CriarCorrida()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            AccountService accountService = new();
            string accountId = await accountService.SignupAsync(signup);

            RideService rideService = new();
            RequestRideModel requestRideModel = new(accountId, 12, 40, -10, -29);
            await rideService.RequestRideAsync(requestRideModel);

            var ex = await Assert.ThrowsAsync<Exception>(() => rideService.RequestRideAsync(requestRideModel));
            Assert.Equal("Passenger has another active ride", ex.Message);
        }

        [Fact()]
        public async Task Quando_RequestValida_Deve_CriarCorrida()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            AccountService accountService = new();
            string accountId = await accountService.SignupAsync(signup);

            RideService rideService = new();
            RequestRideModel requestRideModel = new(accountId, 12, -40, 10, -20);
            string rideId = await rideService.RequestRideAsync(requestRideModel);

            var ride = await rideService.GetRide(rideId);

            Assert.NotNull(ride);
            Assert.NotNull(ride.RideId);
            Assert.Null(ride.DriverId);
            Assert.Equal(requestRideModel.PassengerId, ride.PassengerId);
            Assert.Equal(RideStatus.Requested.ToString(), ride.Status);
        }

        [Fact()]
        public async Task Quando_IsDriverFalse_NaoDeve_AceitarCorrida()
        {
            SignupModel signup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            AccountService accountService = new();
            string accountId = await accountService.SignupAsync(signup);

            RideService rideService = new();
            RequestRideModel requestRideModel = new(accountId, 12, 40, 10, 29);
            string rideId = await rideService.RequestRideAsync(requestRideModel);
            AcceptRideModel acceptRideModel = new(rideId, accountId);

            var ex = await Assert.ThrowsAsync<Exception>(() => rideService.AcceptRideAsync(acceptRideModel));
            Assert.Equal("Account is not driver", ex.Message);
        }

        [Fact()]
        public async Task Quando_CorridaNotRequested_NaoDeve_AceitarCorrida()
        {
            SignupModel driverSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, true, "AAA9999");
            AccountService accountService = new();
            string driverAccountId = await accountService.SignupAsync(driverSignup);

            RideService rideService = new();
            string fakeRideId = new Guid().ToString();
            AcceptRideModel acceptRideModel = new(fakeRideId, driverAccountId);

            var ex = await Assert.ThrowsAsync<Exception>(() => rideService.AcceptRideAsync(acceptRideModel));
            Assert.Equal("Ride is not requested", ex.Message);
        }

        [Fact()]
        public async Task Quando_MotoristaTemOutraCorrida_NaoDeve_AceitarCorrida()
        {
            SignupModel firstPassengerSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            SignupModel secondPassengerSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            SignupModel driverSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, true, "AAA9999");
            AccountService accountService = new();
            string driverAccountId = await accountService.SignupAsync(driverSignup);
            string firstPassengerAccountId = await accountService.SignupAsync(firstPassengerSignup);
            string secondPassengerAccountId = await accountService.SignupAsync(secondPassengerSignup);

            RideService rideService = new();
            RequestRideModel firstRequestRideModel = new(firstPassengerAccountId, 12, 40, 10, 29);
            RequestRideModel secondRequestRideModel = new(secondPassengerAccountId, 12, 40, 10, 29);
            string firstRideId = await rideService.RequestRideAsync(firstRequestRideModel);
            string secondRideId = await rideService.RequestRideAsync(secondRequestRideModel);

            AcceptRideModel acceptRideModel = new(firstRideId, driverAccountId);
            await rideService.AcceptRideAsync(acceptRideModel);

            AcceptRideModel failAcceptRideModel = new(secondRideId, driverAccountId);

            var ex = await Assert.ThrowsAsync<Exception>(() => rideService.AcceptRideAsync(failAcceptRideModel));
            Assert.Equal("Driver has another active ride", ex.Message);
        }

        [Fact()]
        public async Task Quando_AcceptRideOk_Deve_AceitarCorrida()
        {
            SignupModel passengerSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", true, false, string.Empty);
            SignupModel driverSignup = new("John Doe", $"john.doe{new Random().Next()}@gmail.com", "35450161026", false, true, "AAA9999");
            AccountService accountService = new();
            string driverAccountId = await accountService.SignupAsync(driverSignup);
            string passengerAccountId = await accountService.SignupAsync(passengerSignup);

            RideService rideService = new();
            RequestRideModel requestRideModel = new(passengerAccountId, 12, 40, 10, 29);
            string rideId = await rideService.RequestRideAsync(requestRideModel);
            AcceptRideModel acceptRideModel = new(rideId, driverAccountId);
            await rideService.AcceptRideAsync(acceptRideModel);

            RideModel? ride = await rideService.GetRide(rideId);

            Assert.NotNull(ride);
            Assert.Equal(RideStatus.Accepted.ToString(), ride.Status);
            Assert.Equal(driverAccountId, ride.DriverId);
            Assert.Equal(passengerAccountId, ride.PassengerId);
        }
    }
}
