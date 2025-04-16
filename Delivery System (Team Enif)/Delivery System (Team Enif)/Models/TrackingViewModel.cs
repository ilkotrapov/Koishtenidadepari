using Delivery_System__Team_Enif_.Data.Entities;

namespace Delivery_System__Team_Enif_.Models
{
    public class TrackingViewModel
    {
        public string? TrackingNumber { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
        public string? Status { get; set; }
        public List<LocationHistoryItem>? LocationHistory { get; set; }
        public string? ErrorMessage { get; set; }

        public class LocationHistoryItem
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }
    }
}
