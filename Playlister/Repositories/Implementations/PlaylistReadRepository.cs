using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Playlister.Models;

namespace Playlister.Repositories.Implementations
{
    public class PlaylistReadRepository : IPlaylistReadRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public PlaylistReadRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Playlist>> GetAllAsync()
        {
            await using SqliteConnection conn = _connectionFactory.Connection;

            return await conn.QueryAsync<Playlist>(SqlQueries.Read.Playlists);
        }

        public async Task<IEnumerable<(string, int)>> GetPlaylistsWithMissingTracksAsync()
        {
            await using SqliteConnection conn = _connectionFactory.Connection;

            return await conn.QueryAsync<(string, int)>(SqlQueries.Read.PlaylistsWithMissingTracks);
        }
    }
}
