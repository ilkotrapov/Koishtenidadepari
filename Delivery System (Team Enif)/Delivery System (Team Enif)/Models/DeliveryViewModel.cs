using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

public class DeliveryViewModel 
{

    [ValidateNever]
    public IEnumerable<Delivery> Deliveries { get; set; }

    [DisplayName("Delivery Id")]
    public int Id { get; set; }

    [DisplayName("Package Id")]
    public int PackageId { get; set; }

    [Required(ErrorMessage = "Courier selection is required.")]
    [DisplayName("Courier Id")]
    public string CourierId { get; set; }

    [DisplayName("Pickup Time")]
    [DataType(DataType.DateTime)]
    public DateTime PickupTime { get; set; }

    [DisplayName("Delivery Time")]
    [DataType(DataType.DateTime)]
    public DateTime DeliveryTime { get; set; }


    [DisplayName("Delivery Option")]
    [Required]
    public int DeliveryOptionId { get; set; } = (int)DeliveryOptionEnum.PickUp_DropOffLocalOffice;

    [DisplayName("Delivery Option")]
    [ValidateNever]
    public DeliveryOption DeliveryOption { get; set; }

    [DisplayName("Delivery Option")]
    public DeliveryOptionEnum DeliveryOptionSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryOptions { get; set; }


    [DisplayName("Delivery Type")]
    [Required]
    public int DeliveryTypeId { get; set; } = (int)DeliveryTypeEnum.Standard;

    [DisplayName("Delivery Type")]
    [ValidateNever]
    public DeliveryType DeliveryType { get; set; }

    [DisplayName("Delivery Type")]
    public DeliveryTypeEnum DeliveryTypeSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryTypes { get; set; }


    [DisplayName("Status")]
    [Required]
    public int DeliveryStatusId { get; set; } = (int)DeliveryStatusEnum.Pending;

    [DisplayName("Status")]
    [ValidateNever]
    public DeliveryStatus DeliveryStatus { get; set; }

    [DisplayName("Status")]
    public DeliveryStatusEnum DeliveryStatusSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryStatuses { get; set; }
}
