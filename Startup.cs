using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ShareMe
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);

            services.Configure<AppSettings>(config =>
            {
                config.HostUrl = Configuration["SHAREME_HOST_URL"] ?? config.HostUrl;
                config.AdminKey = Configuration["SHAREME_ADMIN_KEY"] ?? config.AdminKey;
                config.UploadFolder = Configuration["SHAREME_UPLOAD_FOLDER"] ?? config.UploadFolder;
                config.FileRequestPath = Configuration["SHAREME_FILE_REQUEST_PATH"] ?? config.FileRequestPath;
                config.ExtensionBlacklist = Configuration["SHAREME_EXTENSION_BLACKLIST"]?.Split(',') ?? config.ExtensionBlacklist;
            });

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<AppSettings> options)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            FileManager.EnsureDirectory(options.Value.PhysicalUploadPath);

            app.UseMvc()
               .UseStaticFiles()
               .UseStaticFiles(new StaticFileOptions()
               {
                   FileProvider = new PhysicalFileProvider(options.Value.PhysicalUploadPath),
                   RequestPath = new PathString(options.Value.FileRequestPath)
               });
        }
    }
}
