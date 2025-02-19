using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Models
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set;}
        public string Address {get; set;}
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    }
}
