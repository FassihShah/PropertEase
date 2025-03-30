using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Application.Interfaces;
using Application;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;

namespace Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<MyApplicationUser> _userManager;
        private readonly SignInManager<MyApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<MyApplicationUser> userManager, SignInManager<MyApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsSignedIn()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return _signInManager.IsSignedIn(user);
        }

        public async Task<bool> IsAuthenticated()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user == null)
                return false;
            return user.Identity.IsAuthenticated;
        }
        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user.Id;
        }


        public async Task<UserDTO?> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.Select(MapToDto).ToList();
        }

        public async Task<List<UserDTO>> GetAgentsAsync()
        {
            var allUsers = _userManager.Users.ToList();
            var agents = new List<UserDTO>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Agent"))
                {
                    agents.Add(MapToDto(user));
                }
            }
            return agents;
        }

        public async Task<List<UserDTO>> GetAdminsAsync()
        {
            var allUsers = _userManager.Users.ToList();
            var admins = new List<UserDTO>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    admins.Add(MapToDto(user));
                }
            }
            return admins;
        }

        public async Task<bool> LoginUserAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> CreateUserAsync(string fullName, string email, string password, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return false;

            var newUser = new MyApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(newUser, role);
            return true;
        }

        public async Task<bool> CreateAgentAsync(string fullName, string email, string password, string licenseNumber, string agencyName, string region, string phoneNumber)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return false;

            var newAgent = new MyApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                AgentLicenseNumber = licenseNumber,
                AgencyName = agencyName,
                Region = region,
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(newAgent, password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(newAgent, "Agent");
            return true;
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                return false;
            return await _userManager.IsInRoleAsync(user, role);
        }
        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user == null) 
                return false;

            user.FullName = userDto.FullName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.AgentLicenseNumber = userDto.AgentLicenseNumber;
            user.AgencyName = userDto.AgencyName;
            user.Region = userDto.Region;
            user.ProfilePicture = userDto.ProfilePicture;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
        private UserDTO MapToDto(MyApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AgentLicenseNumber = user.AgentLicenseNumber,
                AgencyName = user.AgencyName,
                Region = user.Region,
                ProfilePicture = user.ProfilePicture
            };
        }
        public async Task SeedRolesAndUsers()
        {
            // Define roles
            var roles = new[] { "Admin", "Agent", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create an admin user
            var adminEmail = "admin@propertease.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new MyApplicationUser
                {
                    FullName = "Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
