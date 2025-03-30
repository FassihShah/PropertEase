using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypeService(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<List<PropertyType>> GetAllAsync()
        {
            return await _propertyTypeRepository.GetAllAsync();
        }

        public async Task<PropertyType> GetByIdAsync(int typeId)
        {
            return await _propertyTypeRepository.GetByIdAsync(typeId);
        }
    }
}
