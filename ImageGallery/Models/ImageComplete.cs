using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class ImageComplete : Image
    {

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("camera")]
        public string Camera { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }


        [JsonProperty("full_picture")]
        public string FullPicture { get; set; }
    }
}
