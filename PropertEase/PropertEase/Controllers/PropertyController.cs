using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Entities.ViewModels;
using Infrastructure.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;
using Domain.Interfaces;

namespace PropertEase.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IPropertyService _propertyService;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;
        private readonly IPropertyPurposeService _propertyPurposeService;
        private readonly IPropertyTypeService _propertyTypeService;

        public PropertyController(
            IPropertyService propertyService,
            ILocationService locationService,
            IImageService imageService,
            ICategoryService categoryService,
            IPropertyPurposeService propertyPurposeService,
            IPropertyTypeService propertyTypeService,
            IUserService userService,
            IWebHostEnvironment environment
            )
        {
            _propertyService = propertyService;
            _locationService = locationService;
            _imageService = imageService;
            _categoryService = categoryService;
            _propertyPurposeService = propertyPurposeService;
            _propertyTypeService = propertyTypeService;
            _userService = userService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> AddProperty()
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Purposes = await _propertyPurposeService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(IFormCollection form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
                ViewBag.Categories = await _categoryService.GetAllAsync();
                ViewBag.Purposes = await _propertyPurposeService.GetAllAsync();
                return View();
            }

            var userId = await _userService.GetCurrentUserIdAsync();

            var property = new Property
            {
                Title = form["Title"],
                Description = form["Description"],
                PropertyType = await _propertyTypeService.GetByIdAsync( int.Parse(form["PropertyTypeID"])) ,
                Price = decimal.Parse(form["Price"]),
                Bedrooms = int.Parse(form["Bedrooms"]),
                Bathrooms = int.Parse(form["Bathrooms"]),
                Size = int.Parse(form["Size"]),
                Category = await _categoryService.GetByIdAsync( int.Parse(form["CategoryID"])) ,
                Purpose =  await _propertyPurposeService.GetByIdAsync( int.Parse(form["PurposeID"])),
                SellerId = userId
            };

            await _propertyService.AddAsync(property);

            var location = new Location
            {
                City = form["City"],
                Area = form["Area"],
                Street = form["Street"],
                PropertyId = property.PropertyId
            };

            await _locationService.AddAsync(location);

            foreach (var imageFile in form.Files)
            {
                var image = new Image
                {
                    PropertyId = property.PropertyId,
                    Url = SaveImage(imageFile)
                };
                await _imageService.AddAsync(image);
            }

            return RedirectToAction("PropertyDetails", new { id = property.PropertyId });
        }

        public async Task<IActionResult> PropertyDetails(int id)
        {
            var model = await _propertyService.GetPropertyDetailsAsync(id);

            return View(model);
        }

        public async Task<IActionResult> ListProperties(PropertySearchModel model)
        {
            return RedirectToAction("SearchProperties", "Search", new { model.City, model.PropertyType, model.Category, model.Purpose, model.MinSize, model.MaxSize, model.MinPrice, model.MaxPrice }); 
        }

        private string SaveImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return $"/Uploads/{uniqueFileName}";
            }
            return null;
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);
            return View(property);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _propertyService.DeleteAsync(id);
            return RedirectToAction("MyProperties", "User");
        }
    }
}
