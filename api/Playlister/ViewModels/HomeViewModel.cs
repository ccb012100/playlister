namespace Playlister.ViewModels
{
    public class HomeViewModel
    {
        public required Uri SpotifyAuthUrl { get; init; }

        public string? RequestId { get; init; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
