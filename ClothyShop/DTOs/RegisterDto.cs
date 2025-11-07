using ClothyShop.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClothyShop.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }

        public string Role { get; set; } = Roles.User.ToString();
    }
}
