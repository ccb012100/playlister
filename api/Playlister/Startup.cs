using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Http;
using Playlister.Configuration;
using Playlister.Extensions;
using Playlister.Middleware;

namespace Playlister;

public class Startup
{
    private const string CorsPolicyName = "CorsPolicy";
    private readonly IWebHostEnvironment _environment;
    private readonly string? _namespace;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        _environment = environment;
        _namespace = GetType().Namespace;
    }

    private IConfiguration Configuration { get; }

    /// <summary>
    ///     This follows the old .NET pattern
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCors(o => o.AddPolicy(CorsPolicyName,
                    corsBuilder =>
                    {
                        corsBuilder.WithOrigins("https://localhost:5001").WithMethods("GET", "POST").AllowAnyHeader().AllowCredentials();
                    }
                )
            )
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>())
            .AddConfigOptions(Configuration, _environment)
            .AddHttpContextAccessor()
            .AddMiddleware()
            .AddServices()
            .AddRepositories()
            .Configure<HttpClientFactoryOptions>(options => options.SuppressHandlerScope = true)
            .AddHttpClientWithPollyPolicy()
            .AddRefitClients()
            .AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/Forbidden/";
            });

        services.AddEndpointsApiExplorer().AddSwaggerGen();
        services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp"; });

        DefaultTypeMap.MatchNamesWithUnderscores = true; // set Dapper to be compatible with snake_case table names

        if (_environment.IsDevelopment())
        {
            services.AddDebuggingOptions();
        }

        services.ConfigureFluentMigrator();
    }

    /// <summary>
    ///     This follows the old .NET pattern
    /// </summary>
    public void Configure(
        WebApplication app,
        IHostApplicationLifetime appLifetime
    )
    {
        // Log Application lifetime events
        ILogger<Startup> logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();
        appLifetime.ApplicationStarted.Register(() => OnEvent(logger, "Started"));
        appLifetime.ApplicationStopping.Register(() => OnEvent(logger, "Stopping"));
        appLifetime.ApplicationStopped.Register(() => OnEvent(logger, "Stopped"));

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            // TODO: .UseAntiforgery()
            .UseAuthorization();

        app.MapDefaultControllerRoute();

        if (_environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage()
                .UseSwagger()
                .UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStaticFiles()
            .UseCors(CorsPolicyName)
            .UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict })
            .UseMiddleware<GlobalErrorHandlerMiddleware>()
            .UseMiddleware<TokenValidationMiddleware>()
            .AddEndpoints(Configuration, _environment);

        // ~/app/* URLs will serve up the SPA default page (index.html)
        app.UseSpa(spa => { spa.Options.SourcePath = "/app"; });
    }

    private void OnEvent(ILogger logger, string @event)
    {
        logger.LogInformation("{Namespace} {Event}", _namespace, @event);
    }
}
