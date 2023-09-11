namespace Branas.Models
{
    public enum RideStatus
    {
        Requested,
        Accepted,
        InProgress,
        Cancelled,
        Completed
    }

    public class RideModel
    {
        public string RideId { get; set; }
        public string PassengerId { get; set; }
        public string? DriverId { get; set; }
        public string Status { get; set; }
        public decimal Fare { get; set; }
        public decimal Distance { get; set; }
        public decimal FromLat { get; set; }
        public decimal FromLong { get; set; }
        public decimal ToLat { get; set; }
        public decimal ToLong { get; set; }
        public DateTime Date { get; set; }

        public RideModel(Guid rideId, Guid passengerId, Guid? driverId, string status, decimal fare, decimal distance, decimal fromLat, decimal fromLong, decimal toLat, decimal toLong, DateTime date)
        {
            RideId = rideId.ToString();
            PassengerId = passengerId.ToString();
            DriverId = driverId?.ToString();
            Status = status;
            Fare = fare;
            Distance = distance;
            FromLat = fromLat;
            FromLong = fromLong;
            ToLat = toLat;
            ToLong = toLong;
            Date = date;
        }

        public void SetDriver(string driverId)
        {
            DriverId = driverId;
        }

        public void UpdateStatus(RideStatus status)
        {
            Status = status.ToString();
        }
    }
}
