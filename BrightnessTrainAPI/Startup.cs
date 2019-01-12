using BrightnessTrainAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace BrightnessTrainAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = env;

            this.init();
        }

        private void init()
        {
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
                Directory.CreateDirectory(webRootPath);

            Common.BrightnessTrainedFolder = Path.Combine(webRootPath, this.Configuration["BrightnessTrainedFolder"]);
            if (!Directory.Exists(Common.BrightnessTrainedFolder))
                Directory.CreateDirectory(Common.BrightnessTrainedFolder);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //if (env.IsProduction())
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
            app.UseMvc();
        }
    }
}