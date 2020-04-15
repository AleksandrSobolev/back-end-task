using System.Collections.Generic;
using System.Threading.Tasks;
using ImageGallery.Models;

namespace ImageGallery.Interfaces
{
    public interface IImagesCacheService
    {
        Task<List<ImageModel>> GetImagesAsync();

        Task FillCacheAsync();

        Task<ImageModel> GetImageByIdAsync(string id);
    }
}
