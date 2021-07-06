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

        public async Task<Playlist?> Get(string id)
        {
            await using SqliteConnection conn = _connectionFactory.Connection;
            return await conn.QuerySingleOrDefaultAsync<Playlist>("SELECT * FROM Playlist WHERE id = @Id",
                new {Id = id});
        }

        public async Task<IEnumerable<Playlist>> Get()
        {
            await using SqliteConnection conn = _connectionFactory.Connection;
            return await conn.QueryAsync<Playlist>("SELECT * FROM Playlist");
        }
    }
}
