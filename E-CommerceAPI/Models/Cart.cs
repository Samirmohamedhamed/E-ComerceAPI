using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Models
{
    [PrimaryKey(nameof(ProductId),nameof(ApplicationId))]
    public class Cart
    {
        public int ProductId {  get; set; }
        public Product Product { get; set; } = null!;
        public string ApplicationId { get; set; } = null!;
        public ApplicationUser Application { get; set; } = null!;
        public int Count {  get; set; }
    }
}
