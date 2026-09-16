using Ecommerce.DataAccess;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            ViewBag.filter = filter;

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
        public IActionResult Create(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
        {
            if (ImageFile != null)
            {

                var filename = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                product.MainImg = filename;

            }

            var SavedProduct = dbContext.Products.Add(product);
            dbContext.SaveChanges();

            // add product SubImages 
            if (SubImageFiles != null && SubImageFiles.Count() > 0)
            {
                foreach (var image in SubImageFiles)
                {
                    var filename = Guid.NewGuid().ToString() + "-" + image.FileName;
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_sub_images", filename);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        image.CopyTo(stream);
                    }
                    dbContext.ProductSubImages.Add(new ProductSubImage
                    {
                        ProductId = SavedProduct.Entity.Id,
                        Img = filename
                    });
                }
            }

            // add SubColors
            if (Colors != null && Colors.Count() > 0)
            {
                foreach (var color in Colors)
                {
                    dbContext.ProductColors.Add(new ProductColor
                    {
                        ProductId = SavedProduct.Entity.Id,
                        Color = color
                    });
                }
            }



            dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = dbContext.Products.FirstOrDefault(b => b.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            return View(new ProductVM
            {
                Categories = dbContext.Categories.ToList(),
                Brands = dbContext.Brands.ToList(),
                Product = product,
                SubImages = dbContext.ProductSubImages.Where(e => e.ProductId == id).ToList(),
                Colors = dbContext.ProductColors.Where(e => e.ProductId == id).ToList()
            });
        }



        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
        {
            var productInDb = dbContext.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);


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





            if (SubImageFiles != null && SubImageFiles.Count() > 0)
            {
                var oldSubImages = dbContext.ProductSubImages.Where(e => e.ProductId == productInDb.Id);
                
                //remove from DB
                dbContext.ProductSubImages.RemoveRange(oldSubImages);


                //remove from WWWroot
                foreach (var oldImage in oldSubImages)
                {
                    var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_sub_images", oldImage.Img);

                    if (System.IO.File.Exists(Oldfilepath))
                    {
                        System.IO.File.Delete(Oldfilepath);
                    }
                }

                foreach (var file in SubImageFiles)
                {
                    

                    if (file != null)
                    {
                        //add in wwwroot
                        var filename = Guid.NewGuid() + "-" + file.FileName;
                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_sub_images", filename);
                        using (var stream = System.IO.File.Create(filepath))
                        {
                            file.CopyTo(stream);
                        }

                        // add in DB
                        dbContext.ProductSubImages.Add(new ProductSubImage
                        {
                            ProductId = productInDb.Id,
                            Img = filename,
                        });

                    }

                }
            }



            if (Colors != null && Colors.Count() > 0)
            {
                //remove from DB
                var oldColors = dbContext.ProductColors.Where(e => e.ProductId == productInDb.Id);
                dbContext.ProductColors.RemoveRange(oldColors);


                // save in DB
                foreach (var color in Colors)
                {
                    dbContext.ProductColors.Add(new ProductColor
                    {
                        ProductId = productInDb.Id,
                        Color = color
                    });
                }
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


            var oldSubImages = dbContext.ProductSubImages.Where(e => e.ProductId == product.Id);

        

            //remove from WWWroot
            foreach (var oldImage in oldSubImages)
            {
                var Oldfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_sub_images", oldImage.Img);

                if (System.IO.File.Exists(Oldfile))
                {
                    System.IO.File.Delete(Oldfile);
                }
            }

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
