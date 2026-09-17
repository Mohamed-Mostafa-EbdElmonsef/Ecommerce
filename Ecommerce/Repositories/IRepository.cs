using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public interface IRepository<T> where T : class
    {
          Task<EntityEntry<T>> InsertAsync(T entity);

         void Update(T entity);


         void Delete(T entity);




      




          Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTracked = true
            );





          Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTracked = true
            );



          Task<int> CommitAsync();
    }
}
