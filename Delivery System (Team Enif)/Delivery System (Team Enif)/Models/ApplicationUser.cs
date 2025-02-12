using Microsoft.AspNetCore.Identity;

namespace Delivery_System__Team_Enif_.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Track whether the user is pending approval from an admin
        public bool PendingApproval { get; set; } = true; // By default, users are pending approval
    }
}
