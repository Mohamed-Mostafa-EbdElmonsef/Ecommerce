using Ecommerce.DataAccess;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ecommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbcontext = new ApplicationDbContext();
        public IActionResult Index(ProductFilterVM filter)
        {
            var products = dbcontext.Products.AsQueryable();
            products = products.Include(p=>p.Category);

            //filteration
            if (filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
            }
            if (filter.MinPrice >0)
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
            if (filter.IsHot)
            {
                products = products.Where(p => p.Discount > 40);
            }

            //viewbag 
            ViewBag.categories = dbcontext.Categories;
            ViewBag.brands = dbcontext.Brands;

            //paggination
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count()/8.0);
            ViewBag.CurrentPage = filter.Page;
            products = products.Skip((filter.Page-1)*8).Take(8);

            return View(products.AsEnumerable());
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Welcome()
        {
            return View();
        }
        public IActionResult PersonalInfo()
        {
            List<Person> PersonsInDb = new List<Person>()
            {
                new Person{ Id =1,Name="mohamed",Age=25,Salary=9000 },
                new Person{ Id =2,Name="mostafa",Age=56,Salary=6000 },
                new Person{ Id =3,Name="ahmed",Age=23,Salary=10000 }
                
            };
            var count = PersonsInDb.Count();
            return View(new PersonVM()
            {
                Persons = PersonsInDb,
                Count = count
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
