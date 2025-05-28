using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Shared.Extensions
{
    public static class IHostBuilderExtension
    {
        /// <summary>
        /// Add the api to your host builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        public static IHostBuilder ConfigureApi(this IHostBuilder builder, Action<HostBuilderContext, IServiceCollection> options)
        {
            builder.ConfigureServices((context, services) =>
            {
                //HostConfiguration config = new HostConfiguration(services);

                options(context, services);

                services.AddJsonSerializerOptions();
            });

            return builder;
        }
    }
}
