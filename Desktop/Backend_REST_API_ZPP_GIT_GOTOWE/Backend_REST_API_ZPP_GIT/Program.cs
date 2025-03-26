using Backend_REST_API_ZPP.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_REST_API_ZPP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

         

           
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                   .AllowAnyMethod()
                                   .AllowAnyHeader());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    
            builder.Services.AddControllers()
                .AddNewtonsoftJson();  
            builder.Services.AddHttpClient();


            var app = builder.Build();

         
            app.UseHttpsRedirection();
            app.UseAuthorization();



            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/home/index.html"); 
            });

            app.Run();
        }
    }
}
