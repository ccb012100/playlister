using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Playlister.Extensions;
using Playlister.Middleware;

namespace Playlister
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache()
                .AddCors(o => o.AddPolicy(CorsPolicyName, corsBuilder =>
                {
                    corsBuilder.WithOrigins("https://localhost:5001")
                        .WithMethods("GET", "POST")
                        .AllowAnyHeader()
                        .AllowCredentials();
                }))
                .AddMediatR(Assembly.GetAssembly(typeof(Startup)))
                .AddConfigOptions(Configuration)
                .AddHttpContextAccessor()
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient<HttpLoggingMiddleware>()
                .AddTransient<SpotifyAuthHeaderMiddleware>()
                .AddTransient<HttpQueryStringConversionMiddleware>()
                .AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Playlister", Version = "v1"}); })
                .AddHttpClients();

            services.AddControllers();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime)
        {
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
                app.UseExceptionHandler("/Error")
                    .UseHsts();
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseStaticFiles()
                .UseCors(CorsPolicyName)
                .UseMiddleware<GlobalErrorHandlerMiddleware>()
                .UseMiddleware<TokenValidationMiddleware>()
                .AddEndpoints(Configuration, _environment);

            // ~/app/* URLs will serve up the SPA default page (index.html)
            app.UseSpa(spa => { spa.Options.SourcePath = "/app"; });

            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            appLifetime.ApplicationStarted.Register(() => OnStarted(logger));
            appLifetime.ApplicationStopping.Register(() => OnStopping(logger));
            appLifetime.ApplicationStopped.Register(() => OnStopped(logger));
        }

        private void OnStarted(ILogger logger)
        {
            logger.LogInformation($"{GetType().Namespace} Started");
        }

        private void OnStopping(ILogger logger)
        {
            logger.LogInformation($"{GetType().Namespace} Stopping");
        }

        private void OnStopped(ILogger logger)
        {
            logger.LogInformation($"{GetType().Namespace} Stopped");
        }
    }
}
