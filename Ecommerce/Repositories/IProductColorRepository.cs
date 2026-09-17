using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public interface IProductColorRepository :IRepository<ProductColor>
    {
         void RemoveRange(IEnumerable<ProductColor> productColors);
        
    }
}
