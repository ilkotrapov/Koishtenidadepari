using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
using Delivery_System__Team_Enif_.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class OfficeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The Name is required.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "The Location is required.")]
    public string Location { get; set; }

    [DisplayName("Contact Information")]
    [Required(ErrorMessage = "The Contact Information field is required.")]
    public string ContactInfo { get; set; }

    [DisplayName("Working Hours")]
    [Required(ErrorMessage = "The Working Hours field is required.")]
    public string WorkingHours { get; set; }

    [ValidateNever]
    public IEnumerable<ApplicationUserWithRolesViewModel> Employees { get; set; }

    [ValidateNever]
    public IEnumerable<Office> Offices { get; set; }
}
