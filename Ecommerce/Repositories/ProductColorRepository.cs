namespace Ecommerce.Repositories
{
    public class ProductColorRepository :Repository<ProductColor>,IProductColorRepository
    {
        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.ProductColors.RemoveRange(productColors);
        }
    }
}
