using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Domain.Entities;

namespace PropertEase.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IPropertyService _propertyService;

        public UserController(IUserService userService, IWebHostEnvironment environment, IPropertyService propertyService)
        {
            _userService = userService;
            _environment = environment;
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            var user = await _userService.GetByIdAsync(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserDTO model, IFormFile? profilePicture)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Error");
                return View("Profile", model);
            }

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(profilePicture.FileName)}_{model.Id}{Path.GetExtension(profilePicture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profilePicture.CopyTo(fileStream);
                }
                var imageUrl = $"/Uploads/{uniqueFileName}";
                model.ProfilePicture = new Image { Url = imageUrl };
            }

            var result = await _userService.UpdateUserAsync(model);
            if (!result)
            {
                Console.WriteLine("Error updating user");
                return View("Profile", model);
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> MyProperties()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            var properties = await _propertyService.GetBySellerIdAsync(userId);
            return View(properties);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
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
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            var success = await _userService.CreateUserAsync(fullName, email, password, "User");

            if (success)
                return RedirectToAction("Login");

            return View();
        }
        [HttpGet]
        public IActionResult AgentSignup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgentSignup(string fullName, string email, string password, string licenseNumber, string agencyName, string region, string phoneNumber)
        {
            var success = await _userService.CreateAgentAsync(fullName, email, password, licenseNumber, agencyName, region, phoneNumber);

            if (success)
                return RedirectToAction("Login");

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
