using System.ComponentModel.DataAnnotations;

namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class BaseEntity
    {
            [Key]
            public int Id { get; set; }
    }
}
