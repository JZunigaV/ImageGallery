using ImageGallery.Models;
using ImageGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {

        private readonly IImageService imageService;

        public SearchController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [HttpGet]
        public async Task<List<ImageComplete>> GetImages(string search)
        {
            return await imageService.GetAllPhotos(search);
        }
    }
}
