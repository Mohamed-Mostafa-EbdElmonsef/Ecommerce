using Ecommerce.DataAccess;
using Ecommerce.Repositories;
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
        //private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly IRepository<Product> _productRepository;// = new Repository<Product>();
        private readonly IRepository<Category> _categoryRepository;// = new Repository<Category>();
        private readonly IRepository<Brand> _brandRepository; //= new Repository<Brand>();
        private readonly IProductSubImageRepository _productSubImageRepository; //= new ProductSubImageRepository();
        private readonly IProductColorRepository _productColorRepository;//= new ProductColorRepository();

        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IProductSubImageRepository productSubImageRepository, IProductColorRepository productColorRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productSubImageRepository = productSubImageRepository;
            _productColorRepository = productColorRepository;
        }

        public async Task<IActionResult> Index(ProductFilterVM filter)
        {
            //var products = dbContext.Products.AsQueryable();
            var products = await _productRepository.GetAllAsync(includes: [p => p.Category, p => p.Brand]);
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
            //ViewBag.categories = dbContext.Categories;
            ViewBag.categories = await _categoryRepository.GetAllAsync();
            //ViewBag.brands = dbContext.Brands;
            ViewBag.brands = await _brandRepository.GetAllAsync() ;
            ViewBag.filter = filter;

            //paggination
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;
            products = products.Skip((filter.Page - 1) * 8).Take(8);


            return View(products.AsEnumerable());
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
           // var categories = dbContext.Categories.ToList();
            var categories = await _categoryRepository.GetAllAsync();
            //var brands = dbContext.Brands.ToList();
            var brands = await _brandRepository.GetAllAsync();
            return View(new ProductVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList()
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
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

            //var SavedProduct = dbContext.Products.Add(product);
            var SavedProduct = await _productRepository.InsertAsync(product);
            //dbContext.SaveChanges();
            await _productRepository.CommitAsync();
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
                    //dbContext.ProductSubImages.Add(new ProductSubImage
                    await  _productSubImageRepository.InsertAsync(new ProductSubImage
                    {
                        ProductId = SavedProduct.Entity.Id,
                        Img = filename
                    });
                }
            }

            await _productSubImageRepository.CommitAsync();

            // add SubColors
            if (Colors != null && Colors.Count() > 0)
            {
                foreach (var color in Colors)
                {
                    //dbContext.ProductColors.Add(new ProductColor
                    await _productColorRepository.InsertAsync(new ProductColor
                    {
                        ProductId = SavedProduct.Entity.Id,
                        Color = color
                    });
                }
            }



            //dbContext.SaveChanges();
            await _productColorRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
           // var product = dbContext.Products.FirstOrDefault(b => b.Id == id);
            var product = await _productRepository.GetOneAsync(b => b.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            return View(new ProductVM
            {
                //Categories = dbContext.Categories.ToList(),
                Categories = (await _categoryRepository.GetAllAsync()).ToList(),
                //Brands = dbContext.Brands.ToList(),
                Brands = (await _brandRepository.GetAllAsync()).ToList(),
                Product = product,
                //SubImages = dbContext.ProductSubImages.Where(e => e.ProductId == id).ToList(),
                SubImages = (await _productSubImageRepository.GetAllAsync(e => e.ProductId == id)).ToList(),
                //Colors = dbContext.ProductColors.Where(e => e.ProductId == id).ToList()
                Colors = (await _productColorRepository.GetAllAsync(e => e.ProductId == id)).ToList()
            });
        }



        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<string> Colors)
        {
            //var productInDb = dbContext.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productInDb = await _productRepository.GetOneAsync(b => b.Id == product.Id,IsTracked:false);


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

            _productRepository.Update(product);
            await _productRepository.CommitAsync();



            if (SubImageFiles != null && SubImageFiles.Count() > 0)
            {
                //var oldSubImages = dbContext.ProductSubImages.Where(e => e.ProductId == productInDb.Id);
                var oldSubImages = await _productSubImageRepository.GetAllAsync(e => e.ProductId == productInDb.Id);

                //remove from DB
                //dbContext.ProductSubImages.RemoveRange(oldSubImages);

                _productSubImageRepository.RemoveRange(oldSubImages);
          

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
                        //dbContext.ProductSubImages.Add(new ProductSubImage
                        await _productSubImageRepository.InsertAsync(new ProductSubImage
                        {
                            ProductId = productInDb.Id,
                            Img = filename,
                        });

                    }

                }
            }

            await _productSubImageRepository.CommitAsync();

            if (Colors != null && Colors.Count() > 0)
            {
                //remove from DB
                //var oldColors = dbContext.ProductColors.Where(e => e.ProductId == productInDb.Id);
                var oldColors = await _productColorRepository.GetAllAsync(e => e.ProductId == productInDb.Id);
                //dbContext.ProductColors.RemoveRange(oldColors);
                _productColorRepository.RemoveRange(oldColors);


                // save in DB
                foreach (var color in Colors)
                {
                    //dbContext.ProductColors.Add(new ProductColor
                    await _productColorRepository.InsertAsync(new ProductColor
                    {
                        ProductId = productInDb.Id,
                        Color = color
                    });
                }
            }

            //dbContext.Products.Update(product);
            //dbContext.SaveChanges();


            _productRepository.Update(product);
            await _productRepository.CommitAsync();





            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(int id)
        {
            //var product = dbContext.Products.Find(id);
            var product = await _productRepository.GetOneAsync(p => p.Id == id );
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(Oldfilepath))
            {
                System.IO.File.Delete(Oldfilepath);
            }


            //var oldSubImages = dbContext.ProductSubImages.Where(e => e.ProductId == product.Id);
            var oldSubImages = await _productSubImageRepository.GetAllAsync(e => e.ProductId == product.Id);



            //remove from WWWroot
            foreach (var oldImage in oldSubImages)
            {
                var Oldfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_sub_images", oldImage.Img);

                if (System.IO.File.Exists(Oldfile))
                {
                    System.IO.File.Delete(Oldfile);
                }
            }

            //dbContext.Products.Remove(product);
            _productRepository.Delete(product);
            //dbContext.SaveChanges();
            await _productRepository.CommitAsync();

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> DeleteImg(int productId,string img)
        {
            //var ImgDb = dbContext.ProductSubImages.FirstOrDefault(ps => ps.ProductId == productId && ps.Img == img);
            var ImgDb = await _productSubImageRepository.GetOneAsync(ps => ps.ProductId == productId && ps.Img == img);
            if (ImgDb != null)
            {
                //dbContext.ProductSubImages.Remove(ImgDb);
                _productSubImageRepository.Delete(ImgDb);
                //dbContext.SaveChanges();
                await _productSubImageRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Update), new { id=productId});
        }
    }
}
