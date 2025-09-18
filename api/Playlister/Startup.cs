using System.Text.Json;
using System.Text.Json.Serialization;

using Dapper;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Http;

using Playlister.Middleware;

namespace Playlister;

public class Startup {
    private const string CorsPolicyName = "CorsPolicy";

    /// <summary>
    ///     This follows the old .NET pattern
    /// </summary>
    public static WebApplicationBuilder ConfigureServices( WebApplicationBuilder builder ) {
        builder.Services
            .AddCors(
                o => o.AddPolicy(
                    CorsPolicyName ,
                    corsBuilder => {
                        corsBuilder.WithOrigins( "https://127.0.0.1:5001" ).WithMethods( "GET" , "POST" ).AllowAnyHeader( ).AllowCredentials( );
                    }
                )
            )
            .AddHttpContextAccessor( )
            .AddMiddleware( )
            .AddServices( )
            .AddRepositories( )
            .Configure<HttpClientFactoryOptions>( options => options.SuppressHandlerScope = true )
            .AddHttpClientWithPollyPolicy( )
            .AddRefitClients( )
            .AddControllersWithViews( )
            .AddJsonOptions(
                options => {
                    /*
                     * IMPORTANT: Enums must also be marked with attribute `[JsonConverter( typeof(JsonStringEnumConverter) )]`
                     */
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter( JsonNamingPolicy.SnakeCaseLower , false )
                    );

                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }
            );

        builder.Services
            .AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
            .AddCookie(
                options => {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes( 20 );
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                }
            );

        builder.Services
            .AddEndpointsApiExplorer( )
            .AddSwaggerGen( );

        if ( builder.Environment.IsDevelopment( ) ) {
            builder.Services.AddDebuggingOptions( );
        }

        DefaultTypeMap.MatchNamesWithUnderscores = true; // For compatibility with snake_case table names

        return builder;
    }

    /// <summary>
    ///     This follows the old .NET pattern
    /// </summary>
    public static WebApplication ConfigureWebApplication( WebApplication app ) {
        string @namespace = typeof( Startup ).Namespace!;
        // Log Application lifetime events
        ILogger<Startup> logger = app.Services.GetRequiredService<ILoggerFactory>( ).CreateLogger<Startup>( );
        app.Lifetime.ApplicationStarted.Register( ( ) => OnEvent( logger , "Started" , @namespace ) );
        app.Lifetime.ApplicationStopping.Register( ( ) => OnEvent( logger , "Stopping" , @namespace ) );
        app.Lifetime.ApplicationStopped.Register( ( ) => OnEvent( logger , "Stopped" , @namespace ) );

        app.UseHttpsRedirection( )
            .UseStaticFiles( )
            .UseRouting( )
            .UseAuthentication( )
            // TODO: .UseAntiforgery()
            .UseAuthorization( );

        app.MapDefaultControllerRoute( );

        if ( app.Environment.IsDevelopment( ) ) {
            app.UseDeveloperExceptionPage( )
                .UseSwagger( )
                .UseSwaggerUI( );
        } else {
            app.UseExceptionHandler( "/Home/Error" , true );
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts( );
        }

        app.UseCors( CorsPolicyName )
            .UseCookiePolicy( new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict } )
            .UseMiddleware<GlobalErrorHandlerMiddleware>( )
            .UseMiddleware<TokenValidationMiddleware>( )
            .UseEndpoints(
                endpoints => {
                    if ( app.Environment.IsDevelopment( ) ) {
                        endpoints.MapGet( // view app configuration at /debug
                            "/debug" ,
                            async context =>
                                await context.Response.WriteAsync(
                                    ( app.Configuration as IConfigurationRoot )!.GetDebugView( )
                                )
                        );
                    }

                    endpoints.MapGet( "/info" , async context => await context.Response.WriteAsJsonAsync( new AppInfo( ) ) );
                    endpoints.MapControllers( );
                }
            );

        app.UseStatusCodePagesWithReExecute( "/Home/Error" , "?statusCode={0}" );

        return app;
    }

    private static void OnEvent( ILogger logger , string @event , string @namespace ) {
        logger.LogInformation( "{Namespace} {Event}" , @namespace , @event );
    }
}
