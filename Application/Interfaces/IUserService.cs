using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<string> GetCurrentUserIdAsync();
        Task<UserDTO> GetByIdAsync(string userId);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<List<UserDTO>> GetAgentsAsync();
        Task<List<UserDTO>> GetAdminsAsync();
        Task<bool> UpdateUserAsync(UserDTO userDto);
        Task DeleteUserAsync(string userId);
        Task<bool> LoginUserAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<bool> CreateUserAsync(string fullName, string email, string password, string role);
        Task<bool> CreateAgentAsync(string fullName, string email, string password, string licenseNumber, string agencyName, string region, string phoneNumber);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task<bool> IsSignedIn();
        Task<bool> IsAuthenticated();
        Task SeedRolesAndUsers();    // To seed Admin and Roles 
    }
}
