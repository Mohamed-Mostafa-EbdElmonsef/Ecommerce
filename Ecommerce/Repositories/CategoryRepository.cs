using Ecommerce.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class CategoryRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<EntityEntry<Category>> InsertAsync(Category category)
        {
            return await _context.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }


        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }




        private IQueryable<Category> Query(
            Expression<Func<Category, bool>>? filter = null,
            Expression<Func<Category, object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var categories = _context.Categories.AsQueryable();


            if (filter != null)
            {
                categories = categories.Where(filter);
            }


            if (includes != null)
            {
                foreach (var include in includes)
                {
                    categories = categories.Include(include);
                }
            }

            if (!IsTracked)
            {
                categories = categories.AsNoTracking();
            }

            return categories;
        }

      


        public async Task<IEnumerable<Category>> GetAllAsync(
            Expression<Func<Category,bool>>? filter = null, 
            Expression<Func<Category,object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var categories = Query(filter, includes, IsTracked);

            return await categories.ToListAsync();
        }





        public async Task<Category> GetOneAsync(
            Expression<Func<Category, bool>>? filter = null,
            Expression<Func<Category, object>>[]? includes = null,
            bool IsTracked = true
            )
        {
            var categories = Query(filter, includes, IsTracked);

            return await categories.FirstOrDefaultAsync();
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
