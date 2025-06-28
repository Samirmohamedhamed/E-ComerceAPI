using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.Models
{


    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public decimal Discount { get; set; }
        public int Traffic { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public List<Favorite> Favorites { get; set; } =new List<Favorite>();

    }
}


