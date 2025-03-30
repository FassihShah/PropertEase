using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILocationService
    {
        Task<Location> GetByPropertyAsync(int propertyId);
        Task<List<string>> GetAllCitiesAsync();    
        Task AddAsync(Location location);
        Task DeleteAsync(int locationId);
    }
}
