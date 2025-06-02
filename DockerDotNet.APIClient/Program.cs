using DockerDotNet.Core;
using DockerDotNet.Core.Services;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Extensions;

namespace DockerDotNet.APIClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();//.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            builder.Services.AddScoped<DockerClient>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<StreamHelper>();
            builder.Services.AddScoped<HttpClientHelper>();
            builder.Services.AddScoped<IContainerService, ContainerService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IExecService, ExecService>();
            builder.Services.AddScoped<ISystemService, SystemService>();
            builder.Services.AddScoped<IVolumeService, VolumeService>();
            builder.Services.AddScoped<INetworkService, NetworkService>();
            builder.Services.AddJsonSerializerOptions();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.UseWebSockets();
            app.MapControllers();

            app.Run();
        }
    }
}
