using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
       public DbSet<ApplicationUserOTP>  ApplicationUserOTPs { get; set; }
       public DbSet<Cart>   Carts { get; set; }
        public DbSet<Order>  Orders   { get; set; } 
        public DbSet<OrderItems>    OrderItems { get; set; }
        public DbSet<Favorite>     Favorites { get; set; }
        
    }
}
