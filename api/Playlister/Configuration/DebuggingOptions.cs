namespace Playlister.Configuration;

public record DebuggingOptions
{
    public const string Debugging = "Debugging";

    public bool PrintEnvironmentInfo { get; init; }

    public bool UseLoggingBehavior { get; init; }

    public bool UseHttpLoggingMiddleware { get; init; }
}