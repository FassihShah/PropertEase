using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Image>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Images.Where(i => i.PropertyId == propertyId).ToListAsync();
        }

        public async Task AddAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Image image)
        {
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
