using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                await _userService.LogoutAsync();
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }

        public async Task<ActionResult> AllProperties()
        {
            var properties = await _propertyService.GetAllAsync();
            return View(properties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUserId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId) || userId == currentUserId)
            {
                return RedirectToAction("AllUsers");
            }

            await _userService.DeleteUserAsync(userId);
            return RedirectToAction("AllUsers");
        }

        public async Task<IActionResult> ViewUserProperties(string userId)
        {
            var userProperties = await _propertyService.GetBySellerIdAsync(userId);
            return View("ViewUserProperties", userProperties);
        }

        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin(string FullName, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                return View();
            }

            var success = await _userService.CreateUserAsync(FullName.Trim(), Email.Trim(), Password, "Admin");
            if (success)
                return RedirectToAction("AllAdmins");

            ModelState.AddModelError(string.Empty, "Unable to create admin account.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProperty(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

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
