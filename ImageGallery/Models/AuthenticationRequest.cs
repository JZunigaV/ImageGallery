using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class AuthenticationRequest
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }
    }
}
