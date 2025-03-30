using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PropertyRepository:IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Property> GetByIdAsync(int propertyId)
        {
            return await _context.Properties.Include(p => p.Location).Include(p => p.PropertyType).Include(p => p.Category).Include(p => p.Purpose).Where(p=> p.PropertyId == propertyId).FirstOrDefaultAsync();
        }

        public async Task<List<Property>> GetAllAsync()
        {
            return await _context.Properties.Include(p => p.Location).Include(p => p.PropertyType).Include(p => p.Category).Include(p => p.Purpose).ToListAsync();
        }

        public async Task<List<Property>> GetBySellerIdAsync(string sellerId)
        {
            return await _context.Properties.Where(p => p.SellerId == sellerId).Include(p => p.Location).Include(p => p.PropertyType).Include(p => p.Category).Include(p => p.Purpose).ToListAsync();
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }
        }
    }
}
