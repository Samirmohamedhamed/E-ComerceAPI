
using Microsoft.AspNetCore.Identity;

namespace EComerceTestSamir.Repositories
{
    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
    {
        private readonly ApplicationDbContext context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
