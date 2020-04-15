using ImageGallery.Interfaces;
using ImageGallery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageGallery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        // GET api/images
        [HttpGet]
        public async Task<ActionResult<List<ImageModel>>> Get()
        {
            return Ok(await _imagesService.GetAll());
        }

        // GET api/images/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageModel>> Get(string id)
        {
            return Ok(await _imagesService.GetById(id));
        }

        // GET api/images/search/{searchTerm}
        [HttpGet("search/{searchTerm}")]
        public async Task<IEnumerable<ImageModel>> GetAsync(string searchTerm)
        {
            return await _imagesService.Search(searchTerm);
        }
    }
}
