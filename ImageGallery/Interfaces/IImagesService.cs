using System.Collections.Generic;
using System.Threading.Tasks;
using ImageGallery.Models;

namespace ImageGallery.Interfaces
{
    public interface IImagesService
    {
        Task<List<ImageModel>> GetAll();

        Task<ImageModel> GetById(string id);

        Task<List<ImageModel>> Search(string pattern);
    }
}
