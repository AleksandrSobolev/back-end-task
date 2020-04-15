using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageGallery.Models
{
    public class ImagesPageModel
    {
        [JsonProperty("pictures")]
        public List<ImageBaseModel> Images { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public bool HasMore { get; set; }
    }
}
