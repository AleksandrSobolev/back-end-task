using ImageGallery.HostedServices;
using ImageGallery.HttpClients;
using ImageGallery.Interfaces;
using ImageGallery.Models;
using ImageGallery.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery
{
    public static class Container
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<IImagesCacheService, ImagesCacheService>();
            services.AddSingleton<IImagesHttpClient, ImagesHttpClient>();

            services.Configure<ImagesApiSettings>(configuration.GetSection("ImagesApi"));
            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddHostedService<InitImagesCacheHostedService>();
        }
    }
}
