using System.ComponentModel.DataAnnotations;

namespace MovieTime.dto
{
    public class UserRegistretionRequestDto
    {
        public string FullName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
