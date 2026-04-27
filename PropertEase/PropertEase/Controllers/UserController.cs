using Application;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PropertEase.Controllers
{
    public class UserController : Controller
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxProfileImageBytes = 5 * 1024 * 1024;

        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IPropertyService _propertyService;

        public UserController(IUserService userService, IWebHostEnvironment environment, IPropertyService propertyService)
        {
            _userService = userService;
            _environment = environment;
            _propertyService = propertyService;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
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
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserDTO model, IFormFile? profilePicture)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            model.Id = userId;

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var extension = Path.GetExtension(profilePicture.FileName);
                if (profilePicture.Length > MaxProfileImageBytes || !AllowedImageExtensions.Contains(extension))
                {
                    ModelState.AddModelError(string.Empty, "Upload a JPG, PNG, or WEBP image up to 5 MB.");
                    return View("Profile", model);
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    profilePicture.CopyTo(fileStream);
                }

                model.ProfilePicture = new Image { Url = $"/Uploads/{uniqueFileName}" };
            }

            var result = await _userService.UpdateUserAsync(model);
            if (!result)
            {
                return View("Profile", model);
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> MyProperties()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var properties = await _propertyService.GetBySellerIdAsync(userId);
            return View(properties);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var result = await _userService.LoginUserAsync(email, password, rememberMe);

            if (result)
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    if (await _userService.IsUserInRoleAsync(user.Id, "Admin"))
                        return RedirectToAction("Dashboard", "Admin");
                    if (await _userService.IsUserInRoleAsync(user.Id, "Agent"))
                        return RedirectToAction("Profile", "User");

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword)
        {
            if (!PasswordFieldsMatch(password, confirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password do not match.");
                return View();
            }

            var success = await _userService.CreateUserAsync(fullName, email, password, "User");

            if (success)
                return RedirectToAction("Login");

            ModelState.AddModelError(string.Empty, "Unable to create account. Check your details and password strength.");
            return View();
        }

        [HttpGet]
        public IActionResult AgentSignup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgentSignup(string fullName, string email, string password, string confirmPassword, string agentLicenseNumber, string agencyName, string region, string phoneNumber)
        {
            if (!PasswordFieldsMatch(password, confirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password do not match.");
                return View();
            }

            var success = await _userService.CreateAgentAsync(fullName, email, password, agentLicenseNumber, agencyName, region, phoneNumber);

            if (success)
                return RedirectToAction("Login");

            ModelState.AddModelError(string.Empty, "Unable to create agent account. Check your details and password strength.");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }

        private static bool PasswordFieldsMatch(string password, string confirmPassword)
        {
            return !string.IsNullOrWhiteSpace(password) && password == confirmPassword;
        }
    }
}
