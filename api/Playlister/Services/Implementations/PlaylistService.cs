using System.Diagnostics;

using Playlister.Models.SpotifyApi;
using Playlister.Repositories;

namespace Playlister.Services.Implementations;

public class PlaylistService : IPlaylistService {
    private static readonly CacheObject<Playlist> s_playlistCache = new( );
    private static readonly CacheObject<string> s_missingTracksCache = new( );
    private static readonly CacheObject<string> s_updatedPlaylistsCache = new( );
    private readonly ISpotifyApiService _api;
    private readonly ILogger<PlaylistService> _logger;
    private readonly IPlaylistReadRepository _readRepository;
    private readonly IPlaylistWriteRepository _writeRepository;

    public PlaylistService(
        IPlaylistReadRepository readRepository ,
        IPlaylistWriteRepository writeRepository ,
        ISpotifyApiService api ,
        ILogger<PlaylistService> logger
    ) {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _api = api;
        _logger = logger;

        _ = s_playlistCache.Initialize( PopulateCaches );
    }

    #region public

    public async Task<ImmutableArray<Playlist>> GetUserPlaylistsAsync(
        string accessToken ,
        CancellationToken ct
    ) {
        PagingObject<SimplifiedPlaylistObject> page = await _api.GetCurrentUserPlaylistsAsync(
            accessToken ,
            ct
        );

        List<Playlist> lists = page.Items.Select( i => i.ToPlaylist( ) ).ToList( );

        while ( page.Next is not null ) {
            page = await _api.GetCurrentUserPlaylistsAsync( accessToken , page.Next , ct );

            lists.AddRange( page.Items.Select( i => i.ToPlaylist( ) ) );
        }

        return [.. lists];
    }

    public async Task<int> SyncPlaylistsAsync(
        string accessToken ,
        IEnumerable<Playlist> playlists ,
        CancellationToken ct
    ) {
        Stopwatch sw = Stopwatch.StartNew( );

        foreach ( Playlist playlist in playlists.AsParallel( ) ) {
            await UpdatePlaylistAsync( accessToken , playlist , 0 , 50 , ct );
        }

        sw.Stop( );

        if ( s_updatedPlaylistsCache.Items.IsEmpty ) {
            _logger.LogInformation(
                "There were no changed playlists found. Time elapsed: {Elapsed}" ,
                sw.Elapsed.ToDisplayString( )
            );

            return 0;
        }

        int changedPlaylistCount = s_updatedPlaylistsCache.Items.Count;

        _logger.LogInformation(
            "It took {Elapsed} seconds to update the {ChangedPlaylistCount} changed playlists" ,
            sw.Elapsed.ToDisplayString( ) ,
            changedPlaylistCount
        );

        await _writeRepository.TruncateAndPopulatePlaylistAlbum( ct );

        return changedPlaylistCount;
    }

    public async Task SyncPlaylistAsync(
        string accessToken ,
        string playlistId ,
        CancellationToken ct
    ) {
        SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync( accessToken , playlistId , ct );

        Playlist playlist = playlistObject.ToPlaylist( );

        await UpdatePlaylistAsync( accessToken , playlist , 0 , 50 , ct );
    }

    public async Task ForceSyncPlaylistAsync(
        string accessToken ,
        string playlistId ,
        CancellationToken ct
    ) {
        SimplifiedPlaylistObject playlistObject = await _api.GetPlaylistAsync( accessToken , playlistId , ct );

        Playlist playlist = playlistObject.ToPlaylist( );

        await UpdatePlaylistAsync( accessToken , playlist , 0 , 50 , ct , true );
    }

    public async Task<int> DeleteOrphanedPlaylistTracksAsync( CancellationToken ct ) {
        _logger.LogDebug( "Deleting orphaned PlaylistTracks..." );

        return await _writeRepository.DeleteOrphanedPlaylistTracksAsync( ct );
    }

    public async Task<int> RebuildPlaylistAlbumTable( CancellationToken ct ) {
        _logger.LogDebug( "Rebuilding PlaylistAlbum..." );

        return await _writeRepository.TruncateAndPopulatePlaylistAlbum( ct );
    }

    #endregion

    #region private

    /// <summary>
    ///     Update the data for the specified Playlist
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="playlist"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="ct"></param>
    /// <param name="forceSync">If <see langword="true" />, sync the playlist regardless of whether it's changed since the last sync</param>
    /// <returns></returns>
    private async Task UpdatePlaylistAsync(
        string accessToken ,
        Playlist playlist ,
        int offset ,
        int limit ,
        CancellationToken ct ,
        bool forceSync = false
    ) {
        _logger.LogDebug(
            "{Playlist} Updating playlist..." ,
            playlist.LoggingTag
        );

        if ( !forceSync && IsCurrent( playlist ) && HasAllTracks( playlist ) ) {
            _logger.LogDebug( "{PlaylistTag} playlist is up-to-date. Skipping sync" , playlist.LoggingTag );

            return;
        }

        Stopwatch sw = new( );
        sw.Start( );

        // get first page of playlist items
        PagingObject<PlaylistItem> page = await _api.GetPlaylistTracksAsync(
            accessToken ,
            playlist.Id ,
            offset ,
            limit ,
            ct
        );

        /*
         * NOTE: this takes 10s of seconds to update the largest playlists (once the track count starts getting into
         * the thousands; I would like to update this to only grab changes made after the last sync, but the
         * Spotify API's GetPlaylistItems endpoint does not allow filtering or ordering
         */

        /*
         * TODO,PERF: Grab the first page and then calculate the number of remaining pages based on (total/limit).
         *       Then grab those pages in parallel and combine into a single collection.
         */

        // We want to get all the items for the playlist so that they can be inserted into the repository in a single Transaction
        List<PlaylistItem> allItems = page.Items.ToList( );

        while ( page.Next is not null ) {
            page = await _api.GetPlaylistTracksAsync( accessToken , page.Next , ct );
            allItems.AddRange( page.Items );
        }

        if ( allItems.Count != playlist.Count ) {
            _logger.LogWarning(
                "{PlaylistTag} The number of tracks returned from the API does not match Playlist.Count. Expected: {PlaylistCount}. Actual: {PlaylistTrackCount}" ,
                playlist.LoggingTag ,
                playlist.Count ,
                allItems.Count
            );
        }

        string playlistId = playlist.Id;

        ImmutableArray<PlaylistItem> uniqueTracks = allItems.DistinctBy(
                x =>
                    new DistinctPlaylistTracks.DistinctPlaylistTrack( playlistId , x.Track.Id , x.AddedAt )
            )
            .ToImmutableArray( );

        playlist = playlist with { CountUnique = uniqueTracks.Length };

        _logger.LogInformation(
            "{PlaylistTag} playlist contains {CountUnique} unique tracks (out of {Count})" ,
            playlist.LoggingTag ,
            uniqueTracks.Length ,
            playlist.Count
        );

        await _writeRepository.UpsertAsync( playlist , uniqueTracks , ct );

        // only cache after data has been written to database
        Cache( playlist );
        CacheUpdatedPlaylist( playlist );
        DecacheMissingTracks( playlist );

        sw.Stop( );

        _logger.LogInformation(
            "{PlaylistTag} Updated playlist. Total time: {Elapsed}\n" ,
            playlist.LoggingTag ,
            sw.Elapsed.ToDisplayString( )
        );
    }

    /// <summary>
    ///     Indicates whether the database needs to be synced with Spotify for the specified Playlist
    /// </summary>
    /// <param name="playlist"></param>
    /// <returns></returns>
    private bool IsCurrent( Playlist playlist ) {
        Playlist? cachedPlaylist = GetFromCache( playlist.Id );

        if ( cachedPlaylist is null ) // If the playlist isn't in the cache, then we haven't synced it before
        {
            _logger.LogDebug( "{PlaylistTag} was not found in the cache:\n\tSnapshotId:\t{SnapshotId}" , playlist.LoggingTag , playlist.SnapshotId );

            return false;
        }

        if ( cachedPlaylist.SnapshotId != playlist.SnapshotId ) {
            _logger.LogInformation(
                "{PlaylistTag} has changed since the last update:\n\tSnapshotId:         {SnapshotId}\n\tCached SnapshotId:  {CachedSnapshotId}" ,
                playlist.LoggingTag ,
                playlist.SnapshotId ,
                cachedPlaylist.SnapshotId
            );

            return false;
        }

        // if the SnapshotIds match, it hasn't changed since the last sync
        _logger.LogDebug( "{PlaylistTag} is unchanged since the last sync" , playlist.LoggingTag );

        return true;
    }

    /// <summary>
    ///     Indicates whether the database contains all the PlaylistTracks for the specified Playlist.
    ///     If they do not match, it's most likely because there have been tracks deleted from the Playlist in Spotify.
    ///     Note that we're using a simple heuristic based on count of PlaylistTracks in the database; it's possible for the count to be the same but the
    ///     tracks aren't the correct ones, but that would be a bug in the syncing logic that this does not (and should not) account for.
    /// </summary>
    /// <remarks>
    ///     Right now, this can't be hooked into the normal Playlist update logic because we don't store duplicates in
    ///     Playlists and Spotify's definition of a "duplicate" is not entirely clear (singles seem to be treated as a
    ///     duplicate of the album track in at least some cases).
    /// </remarks>
    /// <param name="playlist"></param>
    /// <returns>
    ///     <see langword="false" /> if the number of <see cref="PlaylistTrack" />s for <paramref name="playlist" /> in the database is less than its
    ///     <see cref="Playlist.Count" /> property; Otherwise, <see langword="true" />
    /// </returns>
    private static bool HasAllTracks( Playlist playlist ) {
        bool found = s_missingTracksCache.Items.ContainsKey( playlist.Id );

        // if the playlist isn't in the cache, it has all its tracks
        return !found;
    }

    #endregion

    #region cache

    private void Cache( Playlist playlist ) {
        Playlist pl = s_playlistCache.Items.AddOrUpdate(
            playlist.Id ,
            playlist ,
            ( _ , b ) => b == null ? throw new ArgumentNullException( nameof( b ) ) : playlist
        );

        _logger.LogTrace( "{PlaylistTag} Added playlist to the cache: {Playlist} {PlayListId}" , playlist.LoggingTag , pl.Name , pl.SnapshotId );
    }

    private Playlist? GetFromCache( string id ) {
        bool found = s_playlistCache.Items.TryGetValue( id , out Playlist? playlist );

        if ( found ) {
            _logger.LogTrace( "Found playlist {PlaylistId} in the cache: {Playlist}" , id , playlist!.Name );
        } else {
            _logger.LogDebug( "Playlist {PlaylistId} was not present in the cache" , id );
        }

        return playlist;
    }

    private void CacheMissingTracks( (string, int) playlistWithCount ) {
        (string playlistId, int count) = playlistWithCount;

        string _ = s_missingTracksCache.Items.AddOrUpdate(
            playlistId ,
            count.ToString( ) ,
            ( _ , b ) => b == null ? throw new ArgumentNullException( nameof( b ) ) : count.ToString( )
        );

        _logger.LogDebug(
            "Added playlist {PlaylistId} to the MissingTracks cache; Count = {CacheItemsCount}" ,
            playlistId ,
            s_missingTracksCache.Items.Count
        );
    }

    private void DecacheMissingTracks( Playlist playlist ) {
        if ( s_missingTracksCache.Items.Remove( playlist.Id , out string? _ ) ) {
            _logger.LogTrace( "{PlaylistTag} Removed {PlaylistId} playlist from the MissingTracks cache" , playlist.LoggingTag , playlist.Id );
        } else {
            _logger.LogTrace( "{PlaylistTag} Playlist {PlaylistId} was not present in the MissingTracks cache" , playlist.LoggingTag , playlist.Id );
        }
    }

    private void CacheUpdatedPlaylist( Playlist playlist ) {
        bool added = s_updatedPlaylistsCache.Items.TryAdd( playlist.Id , playlist.Id );

        if ( added ) {
            _logger.LogTrace( "{PlaylistTag} Added playlist {PlaylistId} to the UpdatedPlaylists cache" , playlist.LoggingTag , playlist.Id );
        } else {
            _logger.LogWarning( "{PlaylistTag} Playlist {PlaylistId} was already in the UpdatedPlaylists cache" , playlist.LoggingTag , playlist.Id );
        }
    }

    /// <summary>
    ///     Populate <see cref="s_playlistCache" /> and <see cref="s_missingTracksCache" />
    /// </summary>
    /// <returns></returns>
    private async Task PopulateCaches( ) {
        List<Task> tasks = [
            PopulatePlaylistCache( ) ,
            PopulateMissingTracksCache( )
        ];

        await Task.WhenAll( tasks );

        return;

        async Task PopulatePlaylistCache( ) {
            _logger.LogDebug( "Populating Playlist cache..." );

            IEnumerable<Playlist> playlists = await _readRepository.GetAllAsync( );

            playlists.AsParallel( ).ForAll( Cache );

            _logger.LogDebug(
                "Playlist cache populated: {CacheItemsCount} items" ,
                s_playlistCache.Items.Count
            );
        }

        async Task PopulateMissingTracksCache( ) {
            _logger.LogDebug( "Populating MissingTracks cache..." );

            IEnumerable<(string, int)> missingTracks = await _readRepository.GetPlaylistsWithMissingTracksAsync( );

            missingTracks.AsParallel( ).ForAll( CacheMissingTracks );

            _logger.LogDebug(
                "MissingTracks cache populated: {CacheItemsCount} items" ,
                s_missingTracksCache.Items.Count
            );
        }
    }

    #endregion
}
