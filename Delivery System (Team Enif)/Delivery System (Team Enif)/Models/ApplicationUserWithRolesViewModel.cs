using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Models
{
    public class ApplicationUserWithRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
    }
}
