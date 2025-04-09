using Delivery_System__Team_Enif_.Models;
using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Package : BaseEntity
    {
        public required string SenderName { get; set; }
        public required string RecipientName { get; set; }
        public required string SenderAddress { get; set; }
        public required string RecipientAddress { get; set; }
        public required decimal Length { get; set; }
        public required decimal Width { get; set; }
        public required decimal Hight { get; set; }
        public required decimal Weight { get; set; }
        public int? OfficeId { get; set; }
        public Office? Office { get; set; }

        public int DeliveryOptionId { get; set; }
        public required DeliveryOption DeliveryOption { get; set; }
        public int DeliveryTypeId { get; set; }
        public required DeliveryType DeliveryType { get; set; }
        public int DeliveryStatusId { get; set; }
        public required DeliveryStatus DeliveryStatus { get; set; } 
        public required DateTime DeliveryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
    }
}