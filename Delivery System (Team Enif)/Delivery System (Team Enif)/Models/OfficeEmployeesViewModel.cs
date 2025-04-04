using Delivery_System__Team_Enif_.Data.Entities;
using Delivery_System__Team_Enif_.Models;

public class OfficeEmployeesViewModel
{
    public int? SelectedOfficeId { get; set; }
    public string SelectedRole { get; set; }
    public List<ApplicationUser> Users { get; set; }
}