namespace Branas.Models
{
    public class AcceptRideModel
    {
        public string RideId { get; set; }
        public string DriverId { get; set; }

        public AcceptRideModel(string rideId, string driverId)
        {
            RideId = rideId;
            DriverId = driverId;
        }
    }
}
