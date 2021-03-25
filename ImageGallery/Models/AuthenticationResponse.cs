using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class AuthenticationResponse
    {
        [JsonProperty("auth")]
        public bool Auth { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

    }
}
