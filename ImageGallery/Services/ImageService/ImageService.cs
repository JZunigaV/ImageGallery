using ImageGallery.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class ImageService : IImageService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IImageWrapperService imageWrapper;
        private bool isCacheLoaded;
        private readonly IConfiguration configuration;

        public ImageService(IMemoryCache memoryCache, IImageWrapperService imageWrapper, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.imageWrapper = imageWrapper;
            this.configuration = configuration;
        }

        public async Task<List<ImageComplete>> GetAllPhotos(string search)
        {
      
            if (!isCacheLoaded || memoryCache.Get("checker") == null)
            {
                isCacheLoaded = false;
                await CreateCache();
            }
                

                           
            if (!memoryCache.TryGetValue("photosIds", out List<string> photosIds))
            {               
                return new List<ImageComplete>();                
            }
                
            List<ImageComplete> foundPhotos = new List<ImageComplete>();
            foreach (string photoId in photosIds)
            {
                if (memoryCache.TryGetValue(photoId, out ImageComplete detailedPhoto) &&
                    ((detailedPhoto.Author ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)  ||
                    (detailedPhoto.Camera ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (detailedPhoto.Tags ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)))
                    foundPhotos.Add(detailedPhoto);
            }

            return foundPhotos;
        }

        private async Task CreateCache()
        {
            if (isCacheLoaded)
            {
                return;
            }

            List<ImagePage> photosPages = await imageWrapper.GetAllPhotosPages();

            double expiredTimeThreshold = Double.Parse(configuration["AppSettings:cacheExpireTimeMinutes"]);

            List<string> photosIds = photosPages.SelectMany(page => page.Pictures.Select(p => p.Id)).ToList();
            memoryCache.Set("photosIds", photosIds);

            List<ImageComplete> detailedPhotos = await imageWrapper.GetAllPhotosDetails(photosIds);
            detailedPhotos.Add(new ImageComplete { Id = "checker" });
            detailedPhotos.ForEach(photo => {
                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
                cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(expiredTimeThreshold);
                cacheExpirationOptions.Priority = CacheItemPriority.Normal;
                memoryCache.Set(photo.Id, photo, cacheExpirationOptions);
            });
            
            isCacheLoaded = true;
        }

    }
}
