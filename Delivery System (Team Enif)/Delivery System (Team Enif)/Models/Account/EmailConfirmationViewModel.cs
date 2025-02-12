using System.ComponentModel.DataAnnotations;
public class EmailConfirmationViewModel
{
    public string UserId { get; set; }
    public string Token { get; set; }
}