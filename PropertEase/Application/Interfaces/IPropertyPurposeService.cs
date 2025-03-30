using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPropertyPurposeService
    {
        Task<List<PropertyPurpose>> GetAllAsync();
        Task<PropertyPurpose?> GetByIdAsync(int purposeId);
    }
}
