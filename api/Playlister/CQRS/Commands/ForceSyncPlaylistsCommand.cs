namespace Playlister.CQRS.Commands;

/// <summary>
///     Command to Sync the specified Playlist, regardless of whether it's changed since the last snapshot.
/// </summary>
public record ForceSyncPlaylistCommand( string AccessToken, string PlaylistId );
