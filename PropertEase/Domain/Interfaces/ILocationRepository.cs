using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location> GetByPropertyIdAsync(int propertyId);
        Task<List<string>> GetDistinctCitiesAsync();
        Task AddAsync(Location location);
        Task DeleteAsync(int id);
    }
}
