using Delivery_System__Team_Enif_.Models;

namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Office : BaseEntity
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string ContactInfo { get; set; }
        public required string WorkingHours { get; set; }

        public ICollection<ApplicationUser> Employees { get; set; }
    }
}
