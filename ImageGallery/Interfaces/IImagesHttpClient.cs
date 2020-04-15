using ImageGallery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageGallery.Interfaces
{
    public interface IImagesHttpClient
    {
        Task<IEnumerable<ImageBaseModel>> FetchImagesPagedAsync();

        Task<ImageModel> FetchImageByIdAsync(string id);
    }
}
