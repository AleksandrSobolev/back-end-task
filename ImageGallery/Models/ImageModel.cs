using Newtonsoft.Json;

namespace ImageGallery.Models
{
    public class ImageModel: ImageBaseModel
    {
        public string Author { get; set; }

        public string Camera { get; set; }

        public string Tags { get; set; }

        [JsonProperty("full_picture")]
        public string FullPictureUrl { get; set; }
    }
}
