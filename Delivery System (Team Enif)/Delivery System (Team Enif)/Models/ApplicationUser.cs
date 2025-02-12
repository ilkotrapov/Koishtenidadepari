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
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    }
}
