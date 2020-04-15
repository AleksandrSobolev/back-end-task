using ImageGallery.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ImageGallery.HostedServices
{
    /// <summary>
    /// We could create timer here and refresh cache automatically.
    /// Now we used service only when App started.
    /// </summary>
    public class InitImagesCacheHostedService : IHostedService
    {
        private readonly IImagesCacheService _imagesCacheService;

        public InitImagesCacheHostedService(IImagesCacheService imagesCacheService)
        {
            _imagesCacheService = imagesCacheService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _imagesCacheService.FillCacheAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
