namespace Branas.Models
{
    public class RequestRideModel
    {
        public string PassengerId { get; set; }
        public decimal FromLat { get; set; }
        public decimal FromLong { get; set; }
        public decimal ToLat { get; set; }
        public decimal ToLong { get; set; }

        public RequestRideModel(string passengerId, decimal fromLat, decimal fromLong, decimal toLat, decimal toLong)
        {
            PassengerId = passengerId;
            FromLat = fromLat;
            FromLong = fromLong;
            ToLat = toLat;
            ToLong = toLong;
        }
    }
}
