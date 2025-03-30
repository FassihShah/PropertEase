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
    public class PropertyPurposeService : IPropertyPurposeService
    {
        private readonly IPropertyPurposeRepository _propertyPurposeRepository;

        public PropertyPurposeService(IPropertyPurposeRepository propertyPurposeRepository)
        {
            _propertyPurposeRepository = propertyPurposeRepository;
        }

        public async Task<List<PropertyPurpose>> GetAllAsync()
        {
            return await _propertyPurposeRepository.GetAllAsync();
        }

        public async Task<PropertyPurpose> GetByIdAsync(int purposeId)
        {
            return await _propertyPurposeRepository.GetByIdAsync(purposeId);
        }
    }
}
