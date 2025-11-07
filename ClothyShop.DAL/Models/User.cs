using ClothyShop.DAL.Attributes;

namespace ClothyShop.DAL.Models
{
    [CollectionName("Users")]
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
    }
}
