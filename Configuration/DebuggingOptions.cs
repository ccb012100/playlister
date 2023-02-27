namespace Playlister.Configuration
{
    public record DebuggingOptions
    {
        public const string Debugging = "Debugging";

        public bool PrintEnvironmentInfo { get; init; } = false;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool UseLoggingBehavior { get; init; } = false;

        public bool UseHttpLoggingMiddleware { get; init; } = false;
    }
}
