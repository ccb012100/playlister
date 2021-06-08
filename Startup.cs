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

namespace Playlister
{
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy(CorsPolicyName, corsBuilder =>
                {
                    corsBuilder.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    // .AllowCredentials();
                }))
                .AddMediatR(Assembly.GetAssembly(typeof(Startup)))
                .AddConfigOptions(Configuration)
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Playlister", Version = "v1"}); })
                .AddHttpClients();

            services.AddControllers();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
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
                .UseMiddleware<TokenValidationMiddleware>()
                .AddEndpoints(Configuration, env);

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
