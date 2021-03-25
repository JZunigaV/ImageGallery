using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class Image
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cropped_picture")]
        public string CroppedPicture { get; set; }
    }

}
