using Playlister.Controllers;

namespace Playlister.ViewModels;

public class HomeViewModel
{
    private const string Index = "Index";

    public Link[] Links { get; } = { new("Sync playlists", Index, SyncController.Name) };

    public record Link(string DisplayText, string Action, string Controller);
}
