using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
using Delivery_System__Team_Enif_.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

public enum DeliveryStatusEnum
{
    Pending = 1,
    Active = 2,
    Completed = 3
}

public enum DeliveryTypeEnum
{
    Standard = 1,
    Express = 2
}

public enum DeliveryOptionEnum
{
    DoorToDoor = 1,
    PickUp_DropOffLocalOffice = 2
}

public class DeliveryViewModel
{
    [ValidateNever]
    public IEnumerable<Delivery> Deliveries { get; set; }

    [DisplayName("Delivery Id")]
    public int Id { get; set; }

    [DisplayName("Package Number")]
    [Required(ErrorMessage = "Package Number is required.")]
    public int PackageId { get; set; }

    [DisplayName("Courier")]
    [Required(ErrorMessage = "Courier is required.")]
    public string CourierId { get; set; }

    [ValidateNever]
    public ApplicationUser Courier { get; set; }

    [DisplayName("Courier")]
    [ValidateNever]
    public string CourierName { get; set; }

    [DisplayName("Pickup Time")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Pickup time is required.")]
    public DateTime PickupTime { get; set; }

    [DisplayName("Delivery Time")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Delivery time is required.")]
    public DateTime DeliveryTime { get; set; }

    [DisplayName("Delivery Option")]
    [Required(ErrorMessage = "Delivery option is required.")]
    public int DeliveryOptionId { get; set; }

    [ValidateNever]
    public DeliveryOption DeliveryOption { get; set; }

    public DeliveryOptionEnum DeliveryOptionSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryOptions { get; set; }

    [DisplayName("Delivery Type")]
    [Required(ErrorMessage = "Delivery type is required.")]
    public int DeliveryTypeId { get; set; }

    [ValidateNever]
    public DeliveryType DeliveryType { get; set; }

    public DeliveryTypeEnum DeliveryTypeSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryTypes { get; set; }

    [DisplayName("Status")]
    [Required(ErrorMessage = "Delivery status is required.")]
    public int DeliveryStatusId { get; set; }

    [ValidateNever]
    public DeliveryStatus DeliveryStatus { get; set; }

    public DeliveryStatusEnum DeliveryStatusSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryStatuses { get; set; }

    // Display-only convenience properties
    [DisplayName("Delivery Type")]
    [ValidateNever]
    public string DeliveryTypeName => DeliveryType?.Name;

    [DisplayName("Delivery Option")]
    [ValidateNever]
    public string DeliveryOptionName => DeliveryOption?.Name;

    [DisplayName("Status")]
    [ValidateNever]
    public string DeliveryStatusName => DeliveryStatus?.Name;
}
