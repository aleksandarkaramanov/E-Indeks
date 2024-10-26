using E_Indeks.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using IronPdf.Extensions.Mvc.Core;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace E_Indeks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            builder.Services.AddSession(); // Add session middleware service
            builder.Services.AddHttpContextAccessor(); // Register HttpContextAccessor


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddLocalization(option =>
            {
                option.ResourcesPath = "Resources";
            });


            builder.Services.AddSingleton<IRazorViewRenderer, RazorViewRenderer>();

            var app = builder.Build();
            //ovaa licenca e so free trial od 30 dena(kreiran e acc)
            License.LicenseKey = "IRONSUITE.KTI1622019.FEIT.UKIM.EDU.MK.31028-C08AB45480-BWX5Q-6SBFNOIHNTHB-W4R3LAMVVGSD-MQ3UDTVWHB4F-5DISCMVYZEV2-ZPK57WGLV2OD-3KVO4ONFUAM2-IF74WX-TMAPZ75K52CMEA-DEPLOYMENT.TRIAL-T2DZY6.TRIAL.EXPIRES.07.MAY.2024";

            app.UseRequestLocalization();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{language=en}/{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}