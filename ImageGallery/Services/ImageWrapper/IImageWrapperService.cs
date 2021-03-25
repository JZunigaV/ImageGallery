using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public interface IImageWrapperService
    {
        Task<List<ImagePage>> GetAllPhotosPages();

        Task<List<ImageComplete>> GetAllPhotosDetails(List<string> photosIds);

    }
}
