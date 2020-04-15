using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageGallery.Models
{
    public class ImageBaseModel
    {
        public string Id { get; set; }

        [JsonProperty("cropped_picture")]
        public string CroppedImageUrl { get; set; }
    }
}
