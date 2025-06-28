using System.Linq.Expressions;

namespace E_CommerceAPI.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> CreateAsync(T entity);

        Task<T?> UpdateAsync(T entity);

        Task<T?> DeleteAsync(T entity);
        Task<bool> CommitAsync();


        T? GetOne(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

         IQueryable<T> Get(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        T? GetFirstOrDefault(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? Includes = null,bool traked = true);
        
    }
}
