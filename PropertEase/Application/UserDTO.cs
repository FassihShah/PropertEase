using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? AgentLicenseNumber { get; set; }
        public string? AgencyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Region { get; set; }
        public Image? ProfilePicture { get; set; }
    }
}
