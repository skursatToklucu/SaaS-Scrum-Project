using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PropTabTabIK.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropTabTabIK.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Kürþat
            services.AddDbContext<ProjectContext>(options =>
            {
                options.UseSqlServer(@"Server=DESKTOP-B77ITTF;Database=ScrumDB;Trusted_Connection=true");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            });

            //Fatma Gürel
            //services.AddDbContext<ProjectContext>(options =>
            //{
            //    options.UseSqlServer(@"Server=DESKTOP-19NARKN;Database=ScrumDB;Trusted_Connection=true");
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //});


            //Emre
            //services.AddDbContext<ProjectContext>(options =>
            //{
            //    options.UseSqlServer(@"Server=DESKTOP-19NARKN;Database=ScrumDB;Trusted_Connection=true");
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //});

            services.AddSession(x => x.IdleTimeout = TimeSpan.FromMinutes(30));

                services.AddHttpContextAccessor();

                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                .AddCookie(options =>
                                {//sisteme login iþlemi gerçekleþmediyse home index yönlendir
                                    options.LoginPath = "/Home/Index";
                                });

                services.AddMvcCore(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()//Sisteme yetkili kullanýcý
                    .Build();
                });

         
         
    }
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        SeedData.Seed(app);

        app.UseRouting();
        app.UseAuthentication();
        app.UseSession();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
            name: "SuperAdministator",
            areaName: "SuperAdministator",
            pattern: "SuperAdministator/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapAreaControllerRoute(
            name: "EmployeeArea",
            areaName: "EmployeeArea",
            pattern: "EmployeeArea/{controller=Home}/{action=Index}/{id?}");


            endpoints.MapAreaControllerRoute(
                name: "SiteAdministator",
                areaName: "SiteAdministator",
                pattern: "SiteAdministator/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
}
