using Ecommerce.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            var categories = dbContext.Categories.AsQueryable();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error_Notification"] = "InvalidData";
                return View(category);
            }
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();


            //cookies
            //Response.Cookies.Append("Successful_Notification","Category Added Successfully");
            TempData["Successful_Notification"] = "Category Added Successfully";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = dbContext.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
                //return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            dbContext.Categories.Update(category);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var category = dbContext.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
                //return NotFound();
            }
            dbContext.Categories.Remove(category);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
