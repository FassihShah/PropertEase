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
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<Location> GetByPropertyAsync(int propertyId)
        {
            return await _locationRepository.GetByPropertyIdAsync(propertyId);
        }

        public async Task<List<string>> GetAllCitiesAsync()
        {
            return await _locationRepository.GetDistinctCitiesAsync();
        }

        public async Task AddAsync(Location location)
        {
            await _locationRepository.AddAsync(location);
        }
        public async Task DeleteAsync(int id)
        {
            await _locationRepository.DeleteAsync(id);
        }
    }
}
