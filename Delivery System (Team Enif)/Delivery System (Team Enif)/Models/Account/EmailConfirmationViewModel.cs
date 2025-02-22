namespace Delivery_System__Team_Enif_.Models.Account
{
    public class EmailConfirmationViewModel
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }
    }
}