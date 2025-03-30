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
    public class PropertyTypeRepository : IPropertyTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropertyType>> GetAllAsync()
        {
            return await _context.PropertyTypes.ToListAsync();
        }

        public async Task<PropertyType> GetByIdAsync(int id)
        {
            return await _context.PropertyTypes.FindAsync(id);
        }
    }
}
