using Ecommerce.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class Repository<T> :IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly DbSet<T> _dpSet;
        public Repository()
        {
            _dpSet = _context.Set<T>();
        }
        public async Task<EntityEntry<T>> InsertAsync(T entity)
        {
            return await _dpSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dpSet.Update(entity);
        }


        public void Delete(T entity)
        {
            _dpSet.Remove(entity);
        }




        private IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var entities = _dpSet.AsQueryable();


            if (filter != null)
            {
                entities = entities.Where(filter);
            }


            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!IsTracked)
            {
                entities = entities.AsNoTracking();
            }

            return entities;
        }

      


        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T,bool>>? filter = null, 
            Expression<Func<T,object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var entities = Query(filter, includes, IsTracked);

            return await entities.ToListAsync();
        }





        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var entities = Query(filter, includes, IsTracked);

            return await entities.FirstOrDefaultAsync();
        }



        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return -1;
            }
            
        }
    }
}
