namespace Playlister.CQRS.Commands;

/// <summary>
///     Command to Sync all Playlists associated with the current user
/// </summary>
public record SyncCurrentUserPlaylistsCommand( string AccessToken ) : Command;
