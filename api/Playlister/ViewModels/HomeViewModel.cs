using System.ComponentModel.DataAnnotations;

namespace Playlister.ViewModels
{
    public class HomeViewModel
    {
        [Required] public required Uri SpotifyAuthUrl { get; init; }

        public string? RequestId { get; init; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
