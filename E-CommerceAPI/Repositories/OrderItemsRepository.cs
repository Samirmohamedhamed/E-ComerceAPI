
namespace E_CommerceAPI.Repositories
{
    public class OrderItemsRepository : Repository<OrderItems>, IOrderItemsRepository
    {
        private readonly ApplicationDbContext context;

        public OrderItemsRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
