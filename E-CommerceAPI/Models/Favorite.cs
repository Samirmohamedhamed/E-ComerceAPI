using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Models
{
    [PrimaryKey(nameof(ProductId), nameof(ApplicationId))]

    public class Favorite
    {
        public string ApplicationId { get; set; } = null!;
        public ApplicationUser  ApplicationUser { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
