namespace Playlister.Configuration
{
    public record DebuggingOptions
    {
        public const string Debugging = "Debugging";

        public bool PrintEnvironmentInfo { get; init; } = false;

        public bool UseLoggingBehavior { get; init; } = false;

        public bool UseHttpLoggingMiddleware { get; init; } = false;
    }
}
