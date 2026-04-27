using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace PropertEase.Controllers
{
    public class PropertyController : Controller
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxImageBytes = 5 * 1024 * 1024;

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
            IWebHostEnvironment environment)
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

        [Authorize(Roles = "Agent,Admin")]
        [HttpGet]
        public async Task<IActionResult> AddProperty()
        {
            await PopulatePropertyLists();
            return View();
        }

        [Authorize(Roles = "Agent,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProperty(IFormCollection form)
        {
            if (!TryReadPropertyForm(form, out var property, out var location, out var errorMessage))
            {
                await PopulatePropertyLists();
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }

            var userId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var propertyType = await _propertyTypeService.GetByIdAsync(int.Parse(form["PropertyTypeID"]));
            var category = await _categoryService.GetByIdAsync(int.Parse(form["CategoryID"]));
            var purpose = await _propertyPurposeService.GetByIdAsync(int.Parse(form["PurposeID"]));
            if (propertyType == null || category == null || purpose == null)
            {
                await PopulatePropertyLists();
                ModelState.AddModelError(string.Empty, "Please choose valid property type, category, and purpose.");
                return View();
            }

            property.PropertyType = propertyType;
            property.Category = category;
            property.Purpose = purpose;
            property.SellerId = userId;

            await _propertyService.AddAsync(property);

            location.PropertyId = property.PropertyId;
            await _locationService.AddAsync(location);

            foreach (var imageFile in form.Files)
            {
                var imageUrl = SaveImage(imageFile);
                if (imageUrl == null)
                {
                    continue;
                }

                await _imageService.AddAsync(new Image
                {
                    PropertyId = property.PropertyId,
                    Url = imageUrl
                });
            }

            return RedirectToAction("PropertyDetails", new { id = property.PropertyId });
        }

        public async Task<IActionResult> PropertyDetails(int id)
        {
            var model = await _propertyService.GetPropertyDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public IActionResult ListProperties(PropertySearchModel model)
        {
            return RedirectToAction("SearchProperties", "Search", new { model.City, model.PropertyType, model.Category, model.Purpose, model.MinSize, model.MaxSize, model.MinPrice, model.MaxPrice });
        }

        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = await _userService.GetCurrentUserIdAsync();
            if (property.SellerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(property);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var userId = await _userService.GetCurrentUserIdAsync();
            if (property.SellerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _propertyService.DeleteAsync(id);
            return User.IsInRole("Admin")
                ? RedirectToAction("AllProperties", "Admin")
                : RedirectToAction("MyProperties", "User");
        }

        private async Task PopulatePropertyLists()
        {
            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Purposes = await _propertyPurposeService.GetAllAsync();
        }

        private static bool TryReadPropertyForm(IFormCollection form, out Property property, out Location location, out string errorMessage)
        {
            property = new Property();
            location = new Location();
            errorMessage = string.Empty;

            if (!decimal.TryParse(form["Price"], NumberStyles.Number, CultureInfo.InvariantCulture, out var price) ||
                !int.TryParse(form["Bedrooms"], out var bedrooms) ||
                !int.TryParse(form["Bathrooms"], out var bathrooms) ||
                !int.TryParse(form["Size"], out var size) ||
                !int.TryParse(form["PropertyTypeID"], out _) ||
                !int.TryParse(form["CategoryID"], out _) ||
                !int.TryParse(form["PurposeID"], out _))
            {
                errorMessage = "Please enter valid property details.";
                return false;
            }

            property.Title = form["Title"].ToString().Trim();
            property.Description = form["Description"].ToString().Trim();
            property.Price = price;
            property.Bedrooms = bedrooms;
            property.Bathrooms = bathrooms;
            property.Size = size;

            location.City = form["City"].ToString().Trim();
            location.Area = form["Area"].ToString().Trim();
            location.Street = form["Street"].ToString().Trim();

            if (string.IsNullOrWhiteSpace(property.Title) ||
                string.IsNullOrWhiteSpace(property.Description) ||
                string.IsNullOrWhiteSpace(location.City) ||
                string.IsNullOrWhiteSpace(location.Area) ||
                string.IsNullOrWhiteSpace(location.Street) ||
                price < 0 || bedrooms < 0 || bathrooms < 0 || size <= 0)
            {
                errorMessage = "Please complete all required property details.";
                return false;
            }

            return true;
        }

        private string? SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0 || file.Length > MaxImageBytes)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedImageExtensions.Contains(extension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                file.CopyTo(fileStream);
            }

            return $"/Uploads/{uniqueFileName}";
        }
    }
}
