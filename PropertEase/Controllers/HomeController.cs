using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;
using Application.Interfaces;

namespace PropertEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IPropertyTypeService _propertyTypeService;

        public HomeController(ILogger<HomeController> logger, ICategoryService categoryService, IPropertyTypeService propertyTypeService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _propertyTypeService = propertyTypeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Cities = new List<string>() { "Lahore", "Karachi", "Islamabad" };
            ViewBag.Categories = (await _categoryService.GetAllAsync()).Select(c => c.Name).ToList();
            ViewBag.Types = (await _propertyTypeService.GetAllAsync()).Select(t => t.Name).ToList();
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitForm(string type, string name, string email, string location, int price, int size)
        {
            TempData["Message"] = "Inquiry submitted successfully!";
            return RedirectToAction("Contact");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
