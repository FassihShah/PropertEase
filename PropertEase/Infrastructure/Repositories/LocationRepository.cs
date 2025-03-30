using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.PropertyId == propertyId);
        }

        public async Task<List<string>> GetDistinctCitiesAsync()
        {
            return await _context.Locations.Select(l => l.City).Distinct().ToListAsync();
        }
        public async Task AddAsync(Location location)
        {
            _context.Locations.AddAsync(location);
        }
        public async Task DeleteAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
        }
    }
}
