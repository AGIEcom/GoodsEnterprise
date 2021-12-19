using AutoMapper;
using GoodsEnterprise.DataAccess.Implementation;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.Model.Models;
using GoodsEnterprise.Model.Models.CustomerModel;
using GoodsEnterprise.Web.Maaper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoodsEnterprise.Web.Customer
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
            services.AddMvc().AddRazorOptions(options =>
            {
                options.PageViewLocationFormats
                       .Add("/Pages/Shared/{0}.cshtml");
            });
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/HomePage", "");
            });
            services.AddDbContext<GoodsEnterpriseContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("GoodsEnterpriseDatabase")));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.PageViewLocationFormats.Add("/Pages/Partials/{0}" + RazorViewEngine.ViewExtension);
            });
            services.AddScoped<IUploadDownloadDA, UploadDownloadDA>();
            services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //app.UseMiddleware<ErrorHandlingMiddleware>(Log.Logger);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.Use(async (context, next) =>
            {
                string CurrentUserIDSession = context.Session.GetString("LoginSession");
                //if (context.Request.Path.Value != null && context.Request.Path.Value != "/" && !context.Request.Path.Value.Contains("/HomePage"))
                //{
                //    if (string.IsNullOrEmpty(CurrentUserIDSession))
                //    {
                //        var path = $"/HomePage";
                //        context.Response.Redirect(path);
                //        return;
                //    }

                //}
                await next();
            });
            app.UseEndpoints(endpoints =>
            {
              //  endpoints.MapControllerRoute("default", "{controller=DataBasePagination}/{action=LoadProductTable}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
