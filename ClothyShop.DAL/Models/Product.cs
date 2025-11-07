using ClothyShop.DAL.Attributes;

namespace ClothyShop.DAL.Models
{
    [CollectionName("Products")]
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } 
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
