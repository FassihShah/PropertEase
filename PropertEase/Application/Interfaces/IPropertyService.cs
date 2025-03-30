using Domain.Entities;
using Domain.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPropertyService
    {
        Task<List<Property>> GetAllAsync();
        Task<Property> GetByIdAsync(int propertyId);
        Task<List<Property>> GetBySellerIdAsync(string sellerId);
        Task<DetailsViewModel> GetPropertyDetailsAsync(int propertyId);
        Task<List<Property>> SearchPropertiesAsync(PropertySearchModel searchModel);
        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(int propertyId);
    }
}
