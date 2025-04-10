using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery_System__Team_Enif_.Models;

namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public string CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public ApplicationUser Courier { get; set; }

        [Required]
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        public int DeliveryOptionId { get; set; }

        [ForeignKey(nameof(DeliveryOptionId))]
        public DeliveryOption DeliveryOption { get; set; }

        [Required]
        public int DeliveryTypeId { get; set; }

        [ForeignKey(nameof(DeliveryTypeId))]
        public DeliveryType DeliveryType { get; set; }

        [Required]
        public int DeliveryStatusId { get; set; }

        [ForeignKey(nameof(DeliveryStatusId))]
        public DeliveryStatus DeliveryStatus { get; set; }
    }
}
