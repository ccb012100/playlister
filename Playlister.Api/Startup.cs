using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Playlister.HttpClients;

namespace Playlister
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMediatR(typeof(Startup))
                .AddConfigOptions(Configuration)
                .AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Playlister.Api", Version = "v1"}); })
                .AddHttpClients(Configuration)
                .AddScoped<ISpotifyApi>(_ => new SpotifyApi(Configuration.Get<SpotifyOptions>().ApiBaseUrl))
                .AddScoped<ISpotifyAuthorizationApi>(_ =>
                    new SpotifyAuthorizationApi(Configuration.Get<SpotifyOptions>().ClientId))
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Playlister.Api v1"));
            }

            // The default HSTS value is 30 days.
            // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts().UseHttpsRedirection().UseRouting().UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    if (env.IsDevelopment())
                    {
                        // view app settings at ~/debug
                        endpoints.MapGet("/debug", async context
                            => await context.Response.WriteAsync((Configuration as IConfigurationRoot).GetDebugView()));
                        endpoints.MapGet("/", async context
                            => await context.Response.WriteAsync($"{GetType().Namespace}\n{DateTime.Now}")
                        );
                    }

                    endpoints.MapGet("/info", async context
                        => await context.Response.WriteAsJsonAsync(new AppInfo()));

                    endpoints.MapControllers();
                });

            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            appLifetime.ApplicationStarted.Register(() => OnStarted(logger));
            appLifetime.ApplicationStopping.Register(() => OnStopping(logger));
            appLifetime.ApplicationStopped.Register(() => OnStopped(logger));
        }

        private void OnStarted(ILogger logger) => logger.LogInformation($"{GetType().Namespace} Started");

        private void OnStopping(ILogger logger) => logger.LogInformation($"{GetType().Namespace} Stopping");

        private void OnStopped(ILogger logger) => logger.LogInformation($"{GetType().Namespace} Stopped");
    }
}
