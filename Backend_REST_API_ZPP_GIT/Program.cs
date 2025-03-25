using Backend_REST_API_ZPP.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_REST_API_ZPP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

         

            // Add controllers
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add controllers for API
            builder.Services.AddControllers()
                .AddNewtonsoftJson();  // Adds extended JSON support
            builder.Services.AddHttpClient();


            var app = builder.Build();

            // Middleware setup
            app.UseHttpsRedirection();
            app.UseAuthorization();


            // Serve static files from wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/home/index.html"); // Home page
            });

            app.Run();
        }
    }
}
