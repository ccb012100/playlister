using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Playlister.Configuration;
using Playlister.Models;
using Playlister.Models.SpotifyAccounts;
using Playlister.Services;

namespace Playlister.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly string _connectionString;
        private readonly ICacheService _cacheService;
        private readonly ILogger<AccessTokenRepository> _logger;

        public AccessTokenRepository(IOptions<DatabaseOptions> options, ICacheService cacheService,
            ILogger<AccessTokenRepository> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _cacheService = cacheService;
            _logger = logger;

            IEnumerable<UserAccessToken> tokens = GetAll().Result;
            _cacheService.PopulateCache(tokens);
        }

        public async Task<UserAccessToken> AddToken(SpotifyAccessToken spotifyToken)
        {
            UserAccessToken userToken = _cacheService.Set(spotifyToken);

            try
            {
                const string sql =
                    "INSERT INTO AccessToken(access_token, refresh_token, expiration) VALUES(@AccessToken, @RefreshToken, @Expiration)";

                await using var conn = new SqliteConnection(_connectionString);
                await conn.ExecuteAsync(sql, userToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Token Insert failed:{Environment.NewLine}{e}");
                _cacheService.RemoveAccessToken(userToken.AccessToken);
                throw;
            }

            return userToken;
        }

        public UserAccessToken? Get(string accessToken) => _cacheService.Get(accessToken);

        private async Task<IEnumerable<UserAccessToken>> GetAll()
        {
            _logger.LogDebug("GetAll");
            await using var conn = new SqliteConnection(_connectionString);
            return await conn.QueryAsync<UserAccessToken>("SELECT * FROM AccessToken");
        }

        public void RemoveAccessToken(string accessToken) => _cacheService.RemoveAccessToken(accessToken);
    }
}
