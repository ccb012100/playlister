using System.Reflection;
using FluentMigrator.Runner;
using MediatR;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS;
using Playlister.Middleware;
using Playlister.Models;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Repositories.Implementations;
using Playlister.Services;
using Playlister.Utilities;
using Refit;

namespace Playlister.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.Configure<DebuggingOptions>(config.GetSection(DebuggingOptions.Debugging));
        }

        return services
            .Configure<SpotifyOptions>(config.GetSection(SpotifyOptions.Spotify))
            .Configure<DatabaseOptions>(config.GetSection(DatabaseOptions.Database));
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<IConnectionFactory, ConnectionFactory>()
            .AddScoped<IPlaylistService, PlaylistService>()
            .AddScoped<IAuthService, AuthService>()
            .AddTransient<IAccessTokenUtility, AccessTokenUtility>();
    }

    public static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        return services.AddTransient<HttpLoggingMiddleware>();
        //.AddTransient<SpotifyAuthHeaderMiddleware>();
    }

    public static void ConfigureFluentMigrator(this IServiceCollection services)
    {
        string connectionString = services
            .BuildServiceProvider()
            .GetService<IOptions<DatabaseOptions>>()!
            .Value.ConnectionString;

        ServiceProvider serviceProvider = services
            .AddFluentMigratorCore()
            .ConfigureRunner(
                c =>
                    c.AddSQLite()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(Assembly.GetExecutingAssembly())
                        .For.All()
            )
            .AddLogging(c => c.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using IServiceScope scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }

    public static IServiceCollection AddRefitClients(this IServiceCollection services)
    {
        IOptions<DebuggingOptions>? debugOptions = services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>();

        services
            .AddRefitClient<ISpotifyAccountsApi>(JsonUtility.SnakeCaseRefitSettings)
            .ConfigureHttpClient(
                (svc, c) => { c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress; }
            )
            .AddHttpLoggingMiddleware(debugOptions)
            .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

        services
            .AddRefitClient<ISpotifyApi>(JsonUtility.SnakeCaseRefitSettings)
            .ConfigureHttpClient(
                (svc, c) => { c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress; }
            )
            .AddHttpLoggingMiddleware(debugOptions)
            .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

        return services;
    }

    public static void ValidateConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<SpotifyOptions>()
            .Bind(builder.Configuration.GetSection(SpotifyOptions.Spotify))
            .ValidateDataAnnotations()
            .Validate(
                o => o.CallbackUrl == new Uri("https://localhost:5001/login")
                     || o.CallbackUrl == new Uri("https://localhost:5001/app/home/login"),
                "CallbackUrl must be one of <https://localhost:5001/app/home/login> or <https://localhost:5001/login>")
            .ValidateOnStart();

        builder.Services.AddOptions<DebuggingOptions>()
            .Bind(builder.Configuration.GetSection(DebuggingOptions.Debugging))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<DatabaseOptions>()
            .Bind(builder.Configuration.GetSection(DatabaseOptions.Database))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static IHttpClientBuilder AddHttpLoggingMiddleware(
        this IHttpClientBuilder httpClientBuilder,
        IOptions<DebuggingOptions>? debugOptions
    )
    {
        return debugOptions is { Value.UseHttpLoggingMiddleware: true }
            ? httpClientBuilder.AddHttpMessageHandler<HttpLoggingMiddleware>()
            : httpClientBuilder;
    }

    public static void AddDebuggingOptions(this IServiceCollection services)
    {
        if (services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>() is { Value.UseLoggingBehavior: true })
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // these need to be added as Transient to prevent DI exceptions in Mediatr
        return services
            .AddTransient<IPlaylistReadRepository, PlaylistReadRepository>()
            .AddTransient<IPlaylistWriteRepository, PlaylistWriteRepository>();
    }

    public static void AddEndpoints(this IApplicationBuilder builder, IConfiguration config, IWebHostEnvironment env)
    {
        builder.UseEndpoints(endpoints =>
        {
            if (env.IsDevelopment())
            {
                endpoints.MapGet( // view app settings at ~/debug
                    "/debug",
                    async context =>
                        await context.Response.WriteAsync(
                            (config as IConfigurationRoot)!.GetDebugView()
                        )
                );
            }

            endpoints.MapGet("/info", async context => await context.Response.WriteAsJsonAsync(new AppInfo()));
            endpoints.MapControllers();
        });
    }

    public static IServiceCollection AddHttpClientWithPollyPolicy(this IServiceCollection services)
    {
        services.AddHttpClient<ISpotifyApiService, SpotifyApiService>()
            .AddPolicyHandler(PollyUtility.RetryAfterPolicy);

        return services;
    }
}
