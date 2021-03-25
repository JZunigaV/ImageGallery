using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class ImagePage
    {

        public List<Image> Pictures { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }


    }
}
