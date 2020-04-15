using ImageGallery.Interfaces;
using ImageGallery.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class ImagesCacheService : IImagesCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IImagesHttpClient _imagesHttpClient;
        private readonly ImagesApiSettings _apiSettings;

        public ImagesCacheService(IMemoryCache cache, IOptions<ImagesApiSettings> apiSettings, IImagesHttpClient imagesHttpClient)
        {
            _cache = cache;
            _imagesHttpClient = imagesHttpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task FillCacheAsync()
        {
            await GetImagesAsync();
        }

        public async Task<List<ImageModel>> GetImagesAsync()
        {
            return await _cache.GetOrCreateAsync("images", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_apiSettings.CacheLifeTimeSec);
                return await GetImagesFromApiAsync();
            });
        }

        public async Task<ImageModel> GetImageByIdAsync(string id)
        {
            return (await GetImagesAsync()).FirstOrDefault(img => img.Id == id);
        }

        private async Task<List<ImageModel>> GetImagesFromApiAsync()
        {
            var images = await _imagesHttpClient.FetchImagesPagedAsync();

            var imagesForCache = await GeImagesInParallel(images.Select(img => img.Id).ToArray());

            return imagesForCache.ToList();
        }

        private async Task<IEnumerable<ImageModel>> GeImagesInParallel(ICollection<string> imageIds)
        {
            var images = new List<ImageModel>();
            var batchSize = 100;
            int numberOfBatches = (int)Math.Ceiling((double)imageIds.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentIds = imageIds.Skip(i * batchSize).Take(batchSize);
                var tasks = currentIds.Select(id => _imagesHttpClient.FetchImageByIdAsync(id));
                images.AddRange(await Task.WhenAll(tasks));
            }

            return images;
        }
    }
}
