using Playlister.Mvc.Controllers;

namespace Playlister.Mvc.ViewModels;

public class HomeViewModel
{
    private const string Index = "Index";

    public Link[] Links { get; } = { new( "Sync playlists", "greenyellow", Index, SyncController.Name ), new( "Me", "peru", Index, MeController.Name ) };

    public record Link( string DisplayText, string BackgroundColor, string Action, string Controller );
}
