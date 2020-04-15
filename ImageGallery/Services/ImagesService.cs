using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Interfaces;
using ImageGallery.Models;

namespace ImageGallery.Services
{
    public class ImagesService : IImagesService
    {
        private readonly IImagesCacheService _imagesCacheService;

        public ImagesService(IImagesCacheService imagesCacheService)
        {
            _imagesCacheService = imagesCacheService;
        }

        public async Task<List<ImageModel>> GetAll()
        {
            return await _imagesCacheService.GetImagesAsync();
        }

        public async Task<ImageModel> GetById(string id)
        {
            return await _imagesCacheService.GetImageByIdAsync(id);
        }

        public async Task<List<ImageModel>> Search(string pattern)
        {
            var imagesFromCache = await GetAll();
            return imagesFromCache.Where(img => 
                (!string.IsNullOrWhiteSpace(img.Tags) && img.Tags.Contains(pattern, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(img.Author) && img.Author.Contains(pattern, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(img.Camera) && img.Camera.Contains(pattern, StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
    }
}
