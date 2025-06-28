

namespace E_CommerceAPI.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext context;

        public BrandRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}

