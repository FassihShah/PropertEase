using Microsoft.AspNetCore.Identity;
using Domain.Entities;

namespace Infrastructure.Identity
{
	public class MyApplicationUser : IdentityUser
	{
		public string FullName { get; set; }
		public string? AgentLicenseNumber { get; set; }
		public string? AgencyName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Region {  get; set; }
        public Image? ProfilePicture { get; set; }
    }
}
