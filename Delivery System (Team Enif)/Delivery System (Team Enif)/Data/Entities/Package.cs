using System.ComponentModel.DataAnnotations.Schema;
using Delivery_System__Team_Enif_.Models;
using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Package : BaseEntity
    {
        public int Id { get; set; }
        public required string SenderName { get; set; }
        public required string RecipientName { get; set; }
        public required string SenderAddress { get; set; }
        public required string RecipientAddress { get; set; }
        public required decimal Length { get; set; }
        public required decimal Width { get; set; }
        public required decimal Hight { get; set; }
        public required decimal Weight { get; set; }
        public int DeliveryOptionId { get; set; }
        public required DeliveryOption DeliveryOption { get; set; }
        public int DeliveryTypeId { get; set; }
        public required DeliveryType DeliveryType { get; set; }
        public int DeliveryStatusId { get; set; }
        public required DeliveryStatus DeliveryStatus { get; set; }
        public required DateTime DeliveryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
        public string TrackingNumber { get; set; }
        public Package()
        {
            TrackingNumber = GenerateTrackingNumber();
        }
        private static string GenerateTrackingNumber()
        {
            return Guid.NewGuid().ToString("N")[..12].ToUpper();
        }

        [Column(TypeName = "decimal(9,6)")]
        public decimal CurrentLatitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal CurrentLongitude { get; set; }

        public List<PackageLocation> LocationHistory { get; set; } = new();
    }

    public class PackageLocation
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        public DateTime Timestamp { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }
    }
}