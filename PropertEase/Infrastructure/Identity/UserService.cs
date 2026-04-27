using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<MyApplicationUser> _userManager;
        private readonly SignInManager<MyApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(
            UserManager<MyApplicationUser> userManager,
            SignInManager<MyApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public Task<bool> IsSignedIn()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return Task.FromResult(user != null && _signInManager.IsSignedIn(user));
        }

        public Task<bool> IsAuthenticated()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return Task.FromResult(user?.Identity?.IsAuthenticated == true);
        }

        public Task<string?> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return Task.FromResult(user == null ? null : _userManager.GetUserId(user));
        }

        public async Task<UserDTO?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<List<UserDTO>> GetAgentsAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();
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
            var allUsers = await _userManager.Users.ToListAsync();
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
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(email.Trim(), password, rememberMe, lockoutOnFailure: true);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> CreateUserAsync(string fullName, string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role) ||
                !await _roleManager.RoleExistsAsync(role))
            {
                return false;
            }

            fullName = fullName.Trim();
            email = email.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return false;
            }

            var newUser = new MyApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, role);
            return roleResult.Succeeded;
        }

        public async Task<bool> CreateAgentAsync(string fullName, string email, string password, string licenseNumber, string agencyName, string region, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(licenseNumber) ||
                string.IsNullOrWhiteSpace(agencyName) ||
                string.IsNullOrWhiteSpace(region) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                !await _roleManager.RoleExistsAsync("Agent"))
            {
                return false;
            }

            fullName = fullName.Trim();
            email = email.Trim();
            licenseNumber = licenseNumber.Trim();
            agencyName = agencyName.Trim();
            region = region.Trim();
            phoneNumber = phoneNumber.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return false;
            }

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
            {
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(newAgent, "Agent");
            return roleResult.Succeeded;
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email.Trim());
            return user == null ? null : MapToDto(user);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user == null)
            {
                return false;
            }

            user.FullName = userDto.FullName?.Trim() ?? user.FullName;
            user.PhoneNumber = userDto.PhoneNumber?.Trim();
            user.AgentLicenseNumber = userDto.AgentLicenseNumber?.Trim();
            user.AgencyName = userDto.AgencyName?.Trim();
            user.Region = userDto.Region?.Trim();
            user.ProfilePicture = userDto.ProfilePicture;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task SeedRolesAndUsers()
        {
            var roles = new[] { "Admin", "Agent", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = _configuration["SeedAdmin:Email"] ?? "admin@propertease.com";
            var adminPassword = _configuration["SeedAdmin:Password"];
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new MyApplicationUser
                {
                    FullName = "Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        private static UserDTO MapToDto(MyApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AgentLicenseNumber = user.AgentLicenseNumber,
                AgencyName = user.AgencyName,
                Region = user.Region,
                ProfilePicture = user.ProfilePicture
            };
        }
    }
}
