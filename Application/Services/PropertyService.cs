using Domain.Entities.ViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Interfaces;

namespace Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPropertyPurposeRepository _propertyPurposeRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IUserService _userService;

        public PropertyService(IPropertyRepository propertyRepository,ILocationRepository locationRepository,IImageRepository imageRepository,ICategoryRepository categoryRepository,IPropertyPurposeRepository propertyPurposeRepository,IPropertyTypeRepository propertyTypeRepository,IUserService userService)
        {
            _propertyRepository = propertyRepository;
            _locationRepository = locationRepository;
            _imageRepository = imageRepository;
            _categoryRepository = categoryRepository;
            _propertyPurposeRepository = propertyPurposeRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _userService = userService;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            return await _propertyRepository.GetAllAsync();
        }

        public async Task<Property> GetByIdAsync(int propertyId)
        {
            return await _propertyRepository.GetByIdAsync(propertyId);
        }

        public async Task<DetailsViewModel> GetPropertyDetailsAsync(int propertyId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null) 
                return null;

            var images = await _imageRepository.GetByPropertyIdAsync(propertyId);

            return new DetailsViewModel
            {
                PropertyId = property.PropertyId,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Size = property.Size,
                PropertyType = property.PropertyType.Name,
                Category = property.Category.Name,
                Purpose = property.Purpose.Name,
                ImagesUrl = images.Select(i => i.Url).ToList(),
                Location = property.Location.ToString(),
                SellerName = (await _userService.GetByIdAsync(property.SellerId)).FullName
            };
        }
        public async Task<List<Property>> GetBySellerIdAsync(string sellerId)
        {
            return await _propertyRepository.GetBySellerIdAsync(sellerId);
        }
        public async Task<List<Property>> SearchPropertiesAsync(PropertySearchModel searchModel)
        {
            var properties = await _propertyRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchModel.City))
                properties = properties.Where(p => p.Location.City == searchModel.City).ToList();

            if (!string.IsNullOrEmpty(searchModel.PropertyType))
                properties = properties.Where(p => p.PropertyType.Name == searchModel.PropertyType).ToList();

            if (!string.IsNullOrEmpty(searchModel.Category))
                properties = properties.Where(p => p.Category.Name == searchModel.Category).ToList();

            if (!string.IsNullOrEmpty(searchModel.Purpose))
                properties = properties.Where(p => p.Purpose.Name == searchModel.Purpose).ToList();

            if (searchModel.MinSize.HasValue)
                properties = properties.Where(p => p.Size >= searchModel.MinSize.Value).ToList();

            if (searchModel.MaxSize.HasValue)
                properties = properties.Where(p => p.Size <= searchModel.MaxSize.Value).ToList();

            if (searchModel.MinPrice.HasValue)
                properties = properties.Where(p => p.Price >= searchModel.MinPrice.Value).ToList();

            if (searchModel.MaxPrice.HasValue)
                properties = properties.Where(p => p.Price <= searchModel.MaxPrice.Value).ToList();

            return properties;
        }

        public async Task AddAsync(Property property)
        {
            await _propertyRepository.AddAsync(property);
        }

        public async Task UpdateAsync(Property property)
        {
            await _propertyRepository.UpdateAsync(property);
        }

        public async Task DeleteAsync(int propertyId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property != null)
            {
                await _propertyRepository.DeleteAsync(propertyId);
            }
        }
    }
}
