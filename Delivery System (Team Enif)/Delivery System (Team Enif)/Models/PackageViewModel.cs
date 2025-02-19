using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
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

public class PackageViewModel
{
    [ValidateNever]
    public IEnumerable<Package> Packages { get; set; }
    public int Id { get; set; }

    [DisplayName("Sender Name")]
    [Required(ErrorMessage = "The Sender Name field is required.")]
    public string SenderName { get; set; }

    [DisplayName("Recipient Name")]
    [Required(ErrorMessage = "The Recipient Name field is required.")]
    public string RecipientName { get; set; }

    [DisplayName("Sender Address")]
    [Required(ErrorMessage = "The Sender Address field is required.")]
    public string SenderAddress { get; set; }

    [DisplayName("Recipient Address")]
    [Required(ErrorMessage = "The Recipient Address field is required.")]
    public string RecipientAddress { get; set; }
    public double Weight { get; set; }
    public string Size { get; set; }

    [DisplayName("Delivery Option")]
    public int DeliveryOptionId { get; set; } = (int)DeliveryOptionEnum.PickUp_DropOffLocalOffice;

    [DisplayName("Delivery Option")]
    [ValidateNever]
    public DeliveryOption DeliveryOption { get; set; }

    [DisplayName("Delivery Option")]
    public DeliveryOptionEnum DeliveryOptionSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryOptions { get; set; }

    [DisplayName("Delivery Type")]
    public int DeliveryTypeId { get; set; } = (int)DeliveryTypeEnum.Standard;

    [DisplayName("Delivery Type")]
    [ValidateNever]
    public DeliveryType DeliveryType { get; set; }

    [DisplayName("Delivery Type")]
    public DeliveryTypeEnum DeliveryTypeSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryTypes { get; set; }

    [DisplayName("Status")]
    public int DeliveryStatusId { get; set; } = (int)DeliveryStatusEnum.Pending;

    [DisplayName("Status")]
    [ValidateNever]
    public DeliveryStatus DeliveryStatus { get; set; }

    [DisplayName("Status")]
    public DeliveryStatusEnum DeliveryStatusSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryStatuses { get; set; }

    [Required(ErrorMessage = "The Delivery date field is required.")]
    [DisplayName("Delivery Date")]
    public DateTime DeliveryDate { get; set; }

}
