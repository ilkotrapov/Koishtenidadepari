using System.ComponentModel.DataAnnotations;

namespace Delivery_System__Team_Enif_.Models.Account
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9\s]+$", ErrorMessage = "The Name field can only contain letters, digits, and spaces.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The Phone  field is required.")]
        [Phone]
        public required string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
}
