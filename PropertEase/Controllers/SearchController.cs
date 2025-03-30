using Microsoft.AspNetCore.Mvc;
using Domain.Entities.ViewModels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;
using Application.Interfaces;

namespace PropertEase.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ILocationService _locationService;
        private readonly ICategoryService _categoryService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IPropertyPurposeService _propertyPurposeService;

        public SearchController(
            IPropertyService propertyService,
            ILocationService locationService,
            ICategoryService categoryService,
            IPropertyTypeService propertyTypeService,
            IPropertyPurposeService propertyPurposeService)
        {
            _propertyService = propertyService;
            _locationService = locationService;
            _categoryService = categoryService;
            _propertyTypeService = propertyTypeService;
            _propertyPurposeService = propertyPurposeService;
        }

        public async Task<IActionResult> SearchProperties(PropertySearchModel searchModel)
        {
            List<Property> filtered = await _propertyService.SearchPropertiesAsync(searchModel);

            ViewBag.Cities = await _locationService.GetAllCitiesAsync();
            ViewBag.Categories = (await _categoryService.GetAllAsync()).Select(c => c.Name).ToList();
            ViewBag.Types = (await _propertyTypeService.GetAllAsync()).Select(t => t.Name).ToList();
            ViewBag.Purposes = (await _propertyPurposeService.GetAllAsync()).Select(p => p.Name).ToList();

            return View("~/Views/Property/ListProperties.cshtml", filtered);
        }

        public async Task<IActionResult> PropertiesList(PropertySearchModel searchModel)
        {
            List<Property> filtered = await _propertyService.SearchPropertiesAsync(searchModel);

            ViewBag.Cities = await _locationService.GetAllCitiesAsync();
            ViewBag.Categories = (await _categoryService.GetAllAsync()).Select(c => c.Name).ToList();
            ViewBag.Types = (await _propertyTypeService.GetAllAsync()).Select(t => t.Name).ToList();
            ViewBag.Purposes = (await _propertyPurposeService.GetAllAsync()).Select(p => p.Name).ToList();

            return PartialView("~/Views/Shared/_PropertiesList.cshtml", filtered);
        }

        public async Task<IActionResult> ListFilters()
        {
            ViewBag.Cities = new List<string> { "Lahore", "Karachi", "Islamabad" };
            ViewBag.Categories = (await _categoryService.GetAllAsync()).Select(c => c.Name).ToList();
            ViewBag.Types = (await _propertyTypeService.GetAllAsync()).Select(t => t.Name).ToList();
            ViewBag.Purposes = (await _propertyPurposeService.GetAllAsync()).Select(p => p.Name).ToList();

            return View();
        }
    }
}
