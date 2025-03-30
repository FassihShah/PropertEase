using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;
using Domain.Entities;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNet.Identity;
namespace PropertEase.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;

        public AdminController(IUserService userService, IPropertyService propertyService, ILocationService locationService, IImageService imageService)
        {
            _userService = userService;
            _propertyService = propertyService;
            _locationService = locationService;
            _imageService = imageService;
        }

        public async Task<IActionResult> AllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> AllAgents()
        {
            var agents = await _userService.GetAgentsAsync();
            return View(agents);
        }

        public async Task<IActionResult> AllAdmins()
        {
            var admins = await _userService.GetAdminsAsync();
            return View(admins);
        }
        public async Task<ActionResult> Dashboard()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            var user = await _userService.GetByIdAsync(userId);
            return View(user);
        }

        public async Task<ActionResult> AllProperties()
        {
            var properties = await _propertyService.GetAllAsync(); 
            return View(properties);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteUserAsync(userId);
            return RedirectToAction("AllUsers");
        }
        public async Task<IActionResult> ViewUserProperties(string userId)
        {
            var userProperties = await _propertyService.GetBySellerIdAsync(userId); ;
            return View("ViewUserProperties", userProperties);
        }

        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(string FullName, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                return View();
            }

            var success = await _userService.CreateUserAsync(FullName, Email, Password, "Admin");
            if (success)
                return RedirectToAction("AllAdmins");

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProperty(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);

            var location = await _locationService.GetByPropertyAsync(id);
            var images = await _imageService.GetByPropertyAsync(id);

            if (location != null)
            {
                await _locationService.DeleteAsync(location.LocationId);
            }

            foreach (var image in images)
            {
                await _imageService.DeleteAsync(image.ImageId);
            }

            await _propertyService.DeleteAsync(id);

            return RedirectToAction("AllProperties");
        }


    }
}
