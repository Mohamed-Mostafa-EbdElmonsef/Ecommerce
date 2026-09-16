namespace Ecommerce.ViewModels
{
    public class ProductVM
    {
        public List<Category> Categories { get; set; }
        public List<Brand> Brands { get; set; }
        public Product? Product { get; set; }
        public List<ProductSubImage>? SubImages { get; set; }
        public List<ProductColor>? Colors { get; set; }  
    }
}
