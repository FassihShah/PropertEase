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
    public class PropertyPurposeRepository : IPropertyPurposeRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyPurposeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropertyPurpose>> GetAllAsync()
        {
            return await _context.PropertyPurposes.ToListAsync();
        }

        public async Task<PropertyPurpose> GetByIdAsync(int id)
        {
            return await _context.PropertyPurposes.FindAsync(id);
        }
    }
}
