using System.ComponentModel.DataAnnotations;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set;}
        public string? Address {get; set;}
        public bool isActive { get; set; } = true;

        public int? OfficeId { get; set; }

        public virtual Office Office { get; set; }
    }
}
