using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

public class PackageViewModel
{
    [ValidateNever]
    public IEnumerable<Office> AvailableOffices { get; set; } = new List<Office>();

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
    [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0.")]
    public decimal Length { get; set; }

    [DisplayName("Width (cm)")]
    [Required(ErrorMessage = "The Package Width field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0.")]
    public decimal Width { get; set; }

    [DisplayName("Height (cm)")]
    [Required(ErrorMessage = "The Package Height field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0.")]
    public decimal Hight { get; set; }

    [DisplayName("Package Size (cm³)")]
    public decimal PackageSize => Length * Width * Hight;

    [DisplayName("Weight (kg)")]
    [Required(ErrorMessage = "The Package Weight field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
    public decimal Weight { get; set; }

    [DisplayName("Office")]
    [Required(ErrorMessage = "The Office field is required.")]
    public int OfficeId { get; set; }

    [DisplayName("Office")]
    [ValidateNever]
    public string OfficeSelected { get; set; }

    [DisplayName("Delivery Option")]
    [Required]
    public int DeliveryOptionId { get; set; } = (int)DeliveryOptionEnum.PickUp_DropOffLocalOffice;

    [ValidateNever]
    public DeliveryOption DeliveryOption { get; set; }

    public DeliveryOptionEnum DeliveryOptionSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryOptions { get; set; }

    [DisplayName("Delivery Type")]
    [Required]
    public int DeliveryTypeId { get; set; } = (int)DeliveryTypeEnum.Standard;

    [ValidateNever]
    public DeliveryType DeliveryType { get; set; }

    public DeliveryTypeEnum DeliveryTypeSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryTypes { get; set; }

    [DisplayName("Status")]
    [Required]
    public int DeliveryStatusId { get; set; } = (int)DeliveryStatusEnum.Pending;

    [ValidateNever]
    public DeliveryStatus DeliveryStatus { get; set; }

    public DeliveryStatusEnum DeliveryStatusSelected { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> DeliveryStatuses { get; set; }

    [DisplayName("Delivery Date")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "The Delivery date field is required.")]
    public DateTime DeliveryDate { get; set; }

    [DisplayName("Created Date")]
    [ValidateNever]
    public DateTime CreatedDate { get; set; }

    [ValidateNever]
    public string CreatedByUserId { get; set; }

    [DisplayName("Created By")]
    [ValidateNever]
    public string CreatedByUser { get; set; }

    [ValidateNever]
    public string CurrentUserId { get; set; }
}