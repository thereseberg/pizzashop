using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TomasosPizzeria.IdentityData;
using TomasosPizzeria.Models;

namespace TomasosPizzeria
{
    public class Startup
    {
        //Lokal readonly-variabel
        private readonly IConfiguration _configuration;

        //readonly-variablen sätts via konstruktorn
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddDistributedMemoryCache();
            services.AddSession();

            var conn = _configuration.GetConnectionString("Default");

            services.AddDbContext<TomasosContext>(options => options.UseSqlServer(conn));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "Default",
                template: "{controller=Account}/{action=Main}"
                    );

            });
        }
    }
}
