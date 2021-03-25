using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public interface IImageService
    {
        Task<List<ImageComplete>> GetAllPhotos(string searchTerm);

    }
}
