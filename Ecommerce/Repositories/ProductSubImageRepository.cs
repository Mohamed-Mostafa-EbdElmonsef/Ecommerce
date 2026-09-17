namespace Ecommerce.Repositories
{
    public class ProductSubImageRepository : Repository<ProductSubImage>, IProductSubImageRepository
    {
        public void RemoveRange(IEnumerable<ProductSubImage> productSubImages)
        {
            _context.ProductSubImages.RemoveRange(productSubImages);
        }
    }
}
