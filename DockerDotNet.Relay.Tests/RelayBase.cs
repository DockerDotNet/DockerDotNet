using DockerDotNet.Relay.Services;
using DockerDotNet.Shared.Extensions;
using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DockerDotNet.Relay.Tests
{
    public class RelayBase
    {
        #region Declarations
        
        protected readonly IHost _host;

        #endregion

        #region Constructor

        public RelayBase(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
        }

        #endregion

        #region Setup

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureApi((context, services) =>
            {
                services.AddHttpClient();
                //services.AddSingleton<DockerClient>();
                services.AddSingleton<HttpClientHelper>();
                //services.AddScoped<IImageService, ImageService>();
                services.AddScoped<IContainerService, ContainerRelayService>();
                //services.AddScoped<ISystemService, SystemService>();
                //services.AddScoped<IExecService, ExecService>();
                //services.AddScoped<IVolumeService, VolumeService>();
                services.AddScoped<StreamHelper>();
            });

        #endregion
    }
}