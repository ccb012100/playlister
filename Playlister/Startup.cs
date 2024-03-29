using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Playlister.Extensions;
using Playlister.Middleware;
using Playlister.Utilities;

namespace Playlister
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string? _namespace;
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
            _namespace = GetType().Namespace;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(
                    o =>
                        o.AddPolicy(
                            CorsPolicyName,
                            corsBuilder =>
                            {
                                corsBuilder
                                    .WithOrigins("https://localhost:5001")
                                    .WithMethods("GET", "POST")
                                    .AllowAnyHeader()
                                    .AllowCredentials();
                            }
                        )
                )
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>())
                .AddConfigOptions(Configuration, _environment)
                .AddHttpContextAccessor()
                .AddMiddleware()
                .AddServices()
                .AddRepositories()
                .AddSwaggerGen(
                    c =>
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Playlister", Version = "v1" })
                )
                .Configure<HttpClientFactoryOptions>(options => options.SuppressHandlerScope = true)
                .AddHttpClientWithPollyPolicy()
                .AddRefitClients()
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });

            // set Dapper to be compatible with snake_case table names
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            if (_environment.IsDevelopment())
            {
                services.AddDebuggingOptions();
            }

            services.ConfigureFluentMigrator();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime
        )
        {
            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            appLifetime.ApplicationStarted.Register(() => OnStarted(logger));
            appLifetime.ApplicationStopping.Register(() => OnStopping(logger));
            appLifetime.ApplicationStopped.Register(() => OnStopped(logger));

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Playlister v1");
                        c.RoutePrefix = string.Empty; // serve on ~/
                    });
            }
            else
            {
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/Error").UseHsts();
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseStaticFiles()
                .UseCors(CorsPolicyName)
                .UseMiddleware<GlobalErrorHandlerMiddleware>()
                .UseMiddleware<TokenValidationMiddleware>()
                .AddEndpoints(Configuration, _environment);

            // ~/app/* URLs will serve up the SPA default page (index.html)
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "/app";
            });
        }

        private void OnStarted(ILogger logger)
        {
            logger.LogInformation("{Namespace} Started", _namespace);

            UrlUtility.OpenUrl("https://localhost:5001/app/home", logger);
        }

        private void OnStopping(ILogger logger) =>
            logger.LogInformation("{Namespace} Stopping", _namespace);

        private void OnStopped(ILogger logger) =>
            logger.LogInformation("{Namespace} Stopped", _namespace);
    }
}
