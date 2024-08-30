using System.Collections.Immutable;
using MediatR;
using Playlister.CQRS.Commands;
using Playlister.Models;
using Playlister.Services;

namespace Playlister.CQRS.Handlers;

/// <summary>
///     Add or Update the current user's playlists to the db.
/// </summary>
public class UpdateCurrentUserPlaylistsHandler : IRequestHandler<UpdateCurrentUserPlaylistsCommand, (int total, int updated, int deleted)>
{
    private readonly IPlaylistService _playlistService;

    public UpdateCurrentUserPlaylistsHandler(IPlaylistService playlistService) => _playlistService = playlistService;

    /// <summary>
    ///     Update Current user's playlists
    /// </summary>
    /// <returns>Number of playlists Updated.</returns>
    public async Task<(int total, int updated, int deleted)> Handle(UpdateCurrentUserPlaylistsCommand command, CancellationToken ct)
    {
        ImmutableArray<Playlist> playlists = await _playlistService.GetCurrentUserPlaylistsAsync(command.AccessToken, ct);

        int updated = await _playlistService.UpdatePlaylistsAsync(command.AccessToken, playlists, ct);
        int deleted = await _playlistService.DeleteOrphanedPlaylistTracksAsync(ct);

        return (total: playlists.Length, updated, deleted);
    }
}
