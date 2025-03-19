using DockerDotNet.Core;
using DockerDotNet.Core.Services;
using DockerDotNet.Core.Extensions;

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
            builder.Services.AddScoped<ContainerService>();
            builder.Services.AddScoped<ImageService>();
            builder.Services.AddScoped<ExecService>();
            builder.Services.AddScoped<SystemService>();
            builder.Services.AddScoped<VolumeService>();
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
