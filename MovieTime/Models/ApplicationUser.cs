using Microsoft.AspNetCore.Identity;

namespace MovieTime.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsActivated { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
