using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string Address { get; set; }
    public bool IsEmailVerified { get; set; } = false;
}