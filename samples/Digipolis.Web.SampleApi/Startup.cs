using System;
using System.Reflection;
using AutoMapper;
using Digipolis.Web.SampleApi.Configuration;
using Digipolis.Web.SampleApi.Data;
using Digipolis.Web.SampleApi.Logic;
using Digipolis.Web.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Digipolis.Web.SampleApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddApiExtensions(Configuration.GetSection("ApiExtensions"), x =>
                {
                    //Override settings made by the appsettings.json
                    x.PageSize = 10;
                    x.DisableVersioning = false;
                });

            services.AddGlobalErrorHandling<ApiExceptionMapper>();

            services.AddAuthorization();

            // Add Swagger extensions
            services.AddSwaggerGen<ApiExtensionSwaggerSettings>(o =>
            {
                o.SwaggerDoc(Versions.V1, new OpenApiInfo
                {
                    //Add Inline version
                    Version = Versions.V1,
                    Title = "API V1",
                    Description = "Description for V1 of the API",
                    Contact = new OpenApiContact { Email = "info@digipolis.be", Name = "Digipolis", Url = new Uri("https://www.digipolis.be") },
                    TermsOfService = new Uri("https://www.digipolis.be/tos"),
                    License = new OpenApiLicense
                    {
                        Name = "My License",
                        Url = new Uri("https://www.digipolis.be/licensing")
                    },
                });

                o.SwaggerDoc(Versions.V2, new Version2());
            });

            //Register Dependencies for example project
            services.AddScoped<IValueRepository, ValueRepository>();
            services.AddScoped<IValueLogic, ValueLogic>();

            //Add AutoMapper
            services.AddAutoMapper(new Assembly[] { Assembly.GetEntryAssembly() });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggingBuilder loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Enable Api Extensions
            app.UseApiExtensions();


            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/docs/v1/swagger.json", "V1 Documentation");
                options.SwaggerEndpoint("/docs/v2/swagger.json", "V2 Documentation");
            });

            app.UseSwaggerUiRedirect();

        }
    }
}
