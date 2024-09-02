using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.CQRS.Handlers;
using Playlister.Middleware;
using Playlister.RefitClients;
using Playlister.Repositories;
using Playlister.Repositories.Implementations;
using Playlister.Services;
using Playlister.Utilities;
using Refit;

namespace Playlister.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddServices( this IServiceCollection services )
    {
        return services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<IConnectionFactory, ConnectionFactory>()
            .AddScoped<IPlaylistService, PlaylistService>()
            .AddScoped<IAuthService, AuthService>()
            .AddTransient<IAccessTokenUtility, AccessTokenUtility>()
            .AddHandlers();
    }

    private static IServiceCollection AddHandlers( this IServiceCollection services )
    {
        return services
            .AddTransient<CurrentUserHandler>()
            .AddTransient<PlaylistSyncHandler>()
            .AddTransient<SpotifyAccessTokenHandler>()
            .AddTransient<SpotifyAuthUrlHandler>()
            .AddTransient<SpotifyTokenRefreshHandler>();
    }

    public static IServiceCollection AddMiddleware( this IServiceCollection services )
    {
        return services.AddTransient<HttpLoggingMiddleware>();
    }

    public static void ConfigureFluentMigrator( this IServiceCollection services )
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
                        .WithGlobalConnectionString( connectionString )
                        .ScanIn( Assembly.GetExecutingAssembly() )
                        .For
                        .All()
            )
            .AddLogging( c => c.AddFluentMigratorConsole() )
            .BuildServiceProvider( false );

        using IServiceScope scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }

    public static IServiceCollection AddRefitClients( this IServiceCollection services )
    {
        IOptions<DebuggingOptions>? debugOptions = services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>();

        services
            .AddRefitClient<ISpotifyAccountsApi>( JsonUtility.SnakeCaseRefitSettings )
            .ConfigureHttpClient(
                ( svc, c ) => { c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.AccountsApiBaseAddress; }
            )
            .AddHttpLoggingMiddleware( debugOptions )
            .AddPolicyHandler( PollyUtility.RetryAfterPolicy );

        services
            .AddRefitClient<ISpotifyApi>( JsonUtility.SnakeCaseRefitSettings )
            .ConfigureHttpClient(
                ( svc, c ) => { c.BaseAddress = svc.GetService<IOptions<SpotifyOptions>>()?.Value.ApiBaseAddress; }
            )
            .AddHttpLoggingMiddleware( debugOptions )
            .AddPolicyHandler( PollyUtility.RetryAfterPolicy );

        return services;
    }

    public static WebApplicationBuilder AddAndValidateConfiguration( this WebApplicationBuilder builder )
    {
        builder.Services
            .AddOptions<SpotifyOptions>()
            .Bind( builder.Configuration.GetSection( SpotifyOptions.Spotify ) )
            .ValidateDataAnnotations()
            .Validate(
                o => o.CallbackUrl == new Uri( "https://localhost:5001/login" )
                    || o.CallbackUrl == new Uri( "https://localhost:5001/app/home/login" ),
                "CallbackUrl must be one of <https://localhost:5001/app/home/login> or <https://localhost:5001/login>"
            )
            .ValidateOnStart();

        builder.Services
            .AddOptions<DebuggingOptions>()
            .Bind( builder.Configuration.GetSection( DebuggingOptions.Debugging ) )
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<DatabaseOptions>()
            .Bind( builder.Configuration.GetSection( DatabaseOptions.Database ) )
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
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

    public static void AddDebuggingOptions( this IServiceCollection services )
    {
        if (services.BuildServiceProvider().GetService<IOptions<DebuggingOptions>>() is { Value.UseLoggingBehavior: true })
        {
            // TODO
        }
    }

    public static IServiceCollection AddRepositories( this IServiceCollection services )
    {
        return services
            .AddScoped<IPlaylistReadRepository, PlaylistReadRepository>()
            .AddScoped<IPlaylistWriteRepository, PlaylistWriteRepository>();
    }

    public static IServiceCollection AddHttpClientWithPollyPolicy( this IServiceCollection services )
    {
        services.AddHttpClient<ISpotifyApiService, SpotifyApiService>().AddPolicyHandler( PollyUtility.RetryAfterPolicy );

        return services;
    }
}
