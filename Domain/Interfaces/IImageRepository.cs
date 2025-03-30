using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IImageRepository
    {
        Task<List<Image>> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(Image image);
        Task UpdateAsync(Image image);
        Task DeleteAsync(int id);
    }
}
