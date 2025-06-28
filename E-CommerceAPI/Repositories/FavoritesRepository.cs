
namespace E_CommerceAPI.Repositories
{
    public class FavoritesRepository : Repository<Favorite>, IFavoritesRepository
    {
        private readonly ApplicationDbContext context;

        public FavoritesRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
