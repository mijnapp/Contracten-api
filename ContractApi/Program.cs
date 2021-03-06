using ContractApi.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;
using System.Reflection;

namespace ContractApi
{
    public static class Program
    {
        internal const string ApiVersion = "0.1.0";

        public static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var host = WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices(ConfigureServices)
                .Configure(ConfigureApplication)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                .CreateLogger();

            try
            {
                Log.Information("Starting ContractApi");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ContractApi terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton(context.Configuration);
            services.AddTransient<ReferencedApiService>();
            services.AddTransient<ContractService>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwagger();
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void ConfigureApplication(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }

        private static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("api", new OpenApiInfo
                {
                    Title = "Contracten koppeling",
                    Version = ApiVersion,
                    Description = $"Een standaard voor gegevensuitwisseling tussen MijnApp en de contracten aanbieders.",
                    Contact = new OpenApiContact { Name = "Solviteers Helpdesk" }
                });
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Please enter the ApiKey",
                    Name = "Authorization"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                        },
                        new string[]{}
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private static void UseSwagger(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c => c.RouteTemplate = "{documentName}/swagger.json");
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                {
                    var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
                    var sslPort = config["ASPNETCORE_HTTPS_PORT"];
                    c.SwaggerEndpoint($"https://localhost:{sslPort}/api/swagger.json", $"Contracten koppeling {ApiVersion} (development)");
                }
                c.SwaggerEndpoint("https://raw.githubusercontent.com/mijnapp/Contracten-api/master/api-specification.yaml", $"Contracten koppeling {ApiVersion}");
                c.RoutePrefix = "api";
            });
        }
    }
}
