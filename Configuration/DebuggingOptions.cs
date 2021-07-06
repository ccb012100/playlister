namespace Playlister.Configuration
{
    public record DebuggingOptions
    {
        public const string Debugging = "Debugging";

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool UseLoggingBehavior { get; init; } = false;

        public bool UseHttpLoggingMiddleware { get; init; } = false;
    }
}
