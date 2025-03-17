using DockerDotNet.Core;
using DockerDotNet.Core.Extensions;
using DockerDotNet.Core.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.API.Tests
{
    public class DockerTestBase
    {
        protected readonly IHost _host;

        public DockerTestBase(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureApi((context, services) =>
            {
                services.AddSingleton<DockerClient>();
                services.AddScoped<ImageService>();
                services.AddScoped<ContainerService>();
                services.AddScoped<SystemService>();
                services.AddScoped<ExecService>();
            });
    }
}
