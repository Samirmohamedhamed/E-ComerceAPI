using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        //[MinLength(3)]
        //[MaxLength(15)]
       
        public string Name { get; set; } 
        public string? Description { get; set; }
        public bool Status { get; set; }

        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
