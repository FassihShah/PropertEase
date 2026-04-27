using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class MyApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AgentLicenseNumber { get; set; }
        public string? AgencyName { get; set; }
        public string? Region { get; set; }
        public Image? ProfilePicture { get; set; }
    }
}
