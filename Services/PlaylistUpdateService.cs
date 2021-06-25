using Playlister.Repositories;

namespace Playlister.Services
{
    public class PlaylistUpdateService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IPlaylistTrackRepository _trackRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;

        public PlaylistUpdateService(IPlaylistRepository playlistRepository, IPlaylistTrackRepository trackRepository,
            IArtistRepository artistRepository, IAlbumRepository albumRepository)
        {
            _playlistRepository = playlistRepository;
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
        }
    }
}
