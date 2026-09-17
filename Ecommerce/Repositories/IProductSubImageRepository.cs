using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public interface IProductSubImageRepository : IRepository<ProductSubImage>
    {
         void RemoveRange(IEnumerable<ProductSubImage> productSubImages) ;
    }
}
