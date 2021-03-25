using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    interface IImageService
    {

        

        Task<List<Image>> GetAllPhotosDetails(List<string> photosIds);


    }
}
