using Ecommerce.DataAccess;
using Ecommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        // private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly IRepository<Brand> _brandRepository; // = new Repository<Brand>();

        public BrandController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index()
        {
           //var brands = dbContext.Brands.AsQueryable();
            var brands = await _brandRepository.GetAllAsync();

            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand,IFormFile ImageFile)
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

            //dbContext.Brands.Add(brand);
            await _brandRepository.InsertAsync(brand);
            //dbContext.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var brand = dbContext.Brands.FirstOrDefault(b => b.Id == id);
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage","Home");
            }

            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Brand brand,IFormFile ImageFile)
        {
            //var brandInDb = dbContext.Brands.AsNoTracking().FirstOrDefault(b=>b.Id == brand.Id);
            var brandInDb = await _brandRepository.GetOneAsync(b => b.Id == brand.Id,IsTracked:false);
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
            //dbContext.Brands.Update(brand);
            _brandRepository.Update(brand);
            //dbContext.SaveChanges();
            await _brandRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var brand = dbContext.Brands.Find(id);
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var Oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Logo);

            if (System.IO.File.Exists(Oldfilepath))
            {
                System.IO.File.Delete(Oldfilepath);
            }
            //dbContext.Brands.Remove(brand);
            _brandRepository.Delete(brand);
            //dbContext.SaveChanges ();
            await _brandRepository.CommitAsync();

            return RedirectToAction("Index");
        }
    }
}
