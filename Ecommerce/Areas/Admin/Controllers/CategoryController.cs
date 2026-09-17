using Ecommerce.DataAccess;
using Ecommerce.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly IRepository<Category> _categoryRepository;//= new Repository<Category>();

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var categories = dbContext.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error_Notification"] = "InvalidData";
                return View(category);
            }
            //dbContext.Categories.Add(category);
            await _categoryRepository.InsertAsync(category);

            //dbContext.SaveChanges();
            await _categoryRepository.CommitAsync();


            //cookies
            //Response.Cookies.Append("Successful_Notification","Category Added Successfully");
            TempData["Successful_Notification"] = "Category Added Successfully";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var category = dbContext.Categories.Find(id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
                //return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            // dbContext.Categories.Update(category);
            _categoryRepository.Update(category);
            // dbContext.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            //var category = dbContext.Categories.Find(id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
                //return NotFound();
            }
            // dbContext.Categories.Remove(category);
            _categoryRepository.Delete(category);
            // dbContext.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
