using Playlister.Models.SpotifyApi;

namespace Playlister.CQRS.Commands;

/// <summary>
///     Command to Sync the specified Playlists.
/// </summary>
public record SyncPlaylistsCommand( string AccessToken , IEnumerable<SimplifiedPlaylistObject> Playlists );
