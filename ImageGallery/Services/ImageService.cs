using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class ImageService : IImageService
    {
        public Task<List<Image>> GetAllPhotosDetails(List<string> photosIds)
        {
            throw new NotImplementedException();
        }
    }
}
