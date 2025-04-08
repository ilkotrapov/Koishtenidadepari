using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Delivery_System__Team_Enif_.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using Delivery_System__Team_Enif_.Models; // where ApplicationUser lives


namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Delivery : BaseEntity
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public string CourierId { get; set; }

        [ForeignKey("CourierId")]
        public ApplicationUser Courier { get; set; } // <-- navigation property

        public required DateTime PickupTime { get; set; }
        public required DateTime DeliveryTime { get; set; }

        public int DeliveryOptionId { get; set; }
        public DeliveryOption? DeliveryOption { get; set; }

        public int DeliveryTypeId { get; set; }
        public DeliveryType? DeliveryType { get; set; }

        public int DeliveryStatusId { get; set; }
        public DeliveryStatus? DeliveryStatus { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }

}
