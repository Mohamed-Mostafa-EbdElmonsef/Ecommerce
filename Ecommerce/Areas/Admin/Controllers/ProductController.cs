using Ecommerce.DataAccess;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index(ProductFilterVM filter)
        {
            var products = dbContext.Products.AsQueryable();
            if (filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
            }
            if (filter.MinPrice > 0)
            {
                products = products.Where(p => p.Price >= filter.MinPrice);
            }
            if (filter.MaxPrice > 0)
            {
                products = products.Where(p => p.Price <= filter.MaxPrice);
            }
            if (filter.CategoryId != 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.BrandId != 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
            }
            if (filter.IsLowQuantity)
            {
                products = products.OrderBy(p => p.Quantity);
            }

            //viewbag 
            ViewBag.categories = dbContext.Categories;
            ViewBag.brands = dbContext.Brands;

            //paggination
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;
            products = products.Skip((filter.Page - 1) * 8).Take(8);


            return View(products.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = dbContext.Categories.ToList();
            var brands = dbContext.Brands.ToList();
            return View(new ProductVM()
            {
                Categories = categories,
                Brands = brands
            });
        }
        [HttpPost]
        public IActionResult Create(Product product,IFormFile ImageFile)
        {
            if (ImageFile != null) 
            {
                //var filename = Guid.NewGuid().ToString()+"-"+Path.GetExtension(ImageFile.FileName);
                var filename = Guid.NewGuid().ToString()+"-"+ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images",filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = filename;

            }
                
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = dbContext.Products.FirstOrDefault(b => b.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage","Home");
            }

            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product,IFormFile ImageFile)
        {
            var productInDb = dbContext.Products.AsNoTracking().FirstOrDefault(b=>b.Id == product.Id);
            if (ImageFile != null)
            {

                var filename = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = filename;

                var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productInDb.MainImg);

                if (System.IO.File.Exists(Oldfilepath))
                {
                    System.IO.File.Delete(Oldfilepath);
                }

            }
            else
            {
                product.MainImg = productInDb.MainImg;
            }
            dbContext.Products.Update(product);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = dbContext.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(Oldfilepath))
            {
                System.IO.File.Delete(Oldfilepath);
            }
            dbContext.Products.Remove(product);
            dbContext.SaveChanges ();

            return RedirectToAction("Index");
        }
    }
}
