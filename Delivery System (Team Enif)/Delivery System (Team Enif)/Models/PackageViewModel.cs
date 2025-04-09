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

    [DisplayName("Package number")]
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

    [DisplayName("Length (cm)")]
    [Required(ErrorMessage = "The Package Length field is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "The Package Length must be a positive number more than 0.")]
    public decimal Length { get; set; } = 0;

    [DisplayName("Width (cm)")]
    [Required(ErrorMessage = "The Package Width field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The Package Width must be a positive number more than 0.")]
    public decimal Width { get; set; } = 0;

    [DisplayName("Hight (cm)")]
    [Required(ErrorMessage = "The Package Hight field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The Package Hight must be a positive number more than 0.")]
    public decimal Hight { get; set; } = 0;

    [DisplayName("Package Size (cm³)")]
    public decimal PackageSize => Length * Width * Hight;

    [DisplayName("Weight (kg)")]
    [Required(ErrorMessage = "The Package Weight field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The Package Weight must be a positive number value more than 0")]
    public decimal Weight { get; set; } = 0;

    [DisplayName("Office")]
    [Required(ErrorMessage = "The Office field is required.")]
    public int OfficeId { get; set; }

    [DisplayName("Office")]
    [ValidateNever]
    public string OfficeSelected { get; set; }

    [ValidateNever]
    public IEnumerable<Office> AvailableOffices { get; set; }

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

    [DisplayName("Delivery Date")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "The Delivery date field is required.")]
    public DateTime DeliveryDate { get; set; }

    [ValidateNever]
    [DisplayName("Created Date")]
    public DateTime CreatedDate { get; set; }

    [ValidateNever]
    public string CreatedByUserId { get; set; }

    [ValidateNever]
    [DisplayName("Created By")]
    public string CreatedByUser { get; set; }

    public string CurrentUserId { get; set; }
}
