using Microsoft.AspNetCore.Identity;
namespace Identity.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? City { get; set; }
    }
}
