using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Property> GetByIdAsync(int propertyId);
        Task<List<Property>> GetAllAsync();
        Task<List<Property>> GetBySellerIdAsync(string sellerId);
        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(int propertyId);
    }
}
