using Ecommerce.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            var brands = dbContext.Brands.AsQueryable();

            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand,IFormFile ImageFile)
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
                brand.Logo = filename;

            }
                
            dbContext.Brands.Add(brand);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var brand = dbContext.Brands.FirstOrDefault(b => b.Id == id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage","Home");
            }

            return View(brand);
        }
        [HttpPost]
        public IActionResult Update(Brand brand,IFormFile ImageFile)
        {
            var brandInDb = dbContext.Brands.AsNoTracking().FirstOrDefault(b=>b.Id == brand.Id);
            if (ImageFile != null)
            {

                var filename = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                brand.Logo = filename;

                var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brandInDb.Logo);

                if (System.IO.File.Exists(Oldfilepath))
                {
                    System.IO.File.Delete(Oldfilepath);
                }

            }
            else
            {
                brand.Logo = brandInDb.Logo;
            }
            dbContext.Brands.Update(brand);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var brand = dbContext.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Logo);

            if (System.IO.File.Exists(Oldfilepath))
            {
                System.IO.File.Delete(Oldfilepath);
            }
            dbContext.Brands.Remove(brand);
            dbContext.SaveChanges ();

            return RedirectToAction("Index");
        }
    }
}
