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
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<List<Image>> GetByPropertyAsync(int propertyId)
        {
            return await _imageRepository.GetByPropertyIdAsync(propertyId);
        }

        public async Task AddAsync(Image image)
        {
            await _imageRepository.AddAsync(image);
        }

        public async Task DeleteAsync(int imageId)
        {
            await _imageRepository.DeleteAsync(imageId);
        }
    }
}
