using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class ProductRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? MainImg { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public decimal Discount { get; set; }
        public int Traffic { get; set; }


        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
