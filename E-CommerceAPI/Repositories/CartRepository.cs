
namespace E_CommerceAPI.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
