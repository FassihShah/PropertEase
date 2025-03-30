using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IImageService
    {
        Task<List<Image>> GetByPropertyAsync(int propertyId);
        Task AddAsync(Image image);
        Task DeleteAsync(int imageId);
    }
}
