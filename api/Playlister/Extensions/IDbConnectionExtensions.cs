using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using Dapper;

namespace Playlister.Extensions;

public static class IDbConnectionExtensions
{
    /// <summary>
    ///     Run a parameterized query that upserts <paramref name="items" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="sqlQuery"></param>
    /// <param name="conn"></param>
    /// <param name="dbTxn"></param>
    /// <returns>The number of items affected</returns>
    public static async Task UpsertAsync<T>(
        this IDbConnection conn,
        string sqlQuery,
        ImmutableArray<T> items,
        IDbTransaction dbTxn
    )
    {
        Debug.Assert(!items.IsDefaultOrEmpty);
        Debug.Assert(!string.IsNullOrWhiteSpace(sqlQuery));
        Debug.Assert(conn is not null);
        Debug.Assert(dbTxn is not null);

        await conn.ExecuteAsync(sqlQuery, items, dbTxn);
    }

    /// <summary>
    ///     Run a parameterized query that upserts <paramref name="item" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="conn"></param>
    /// <param name="sqlQuery"></param>
    /// <param name="item"></param>
    /// <param name="dbTxn"></param>
    /// <returns></returns>
    public static async Task UpsertAsync<T>(
        this IDbConnection conn,
        string sqlQuery,
        T item,
        IDbTransaction dbTxn
    )
    {
        Debug.Assert(item is not null);
        Debug.Assert(!string.IsNullOrWhiteSpace(sqlQuery));
        Debug.Assert(conn is not null);
        Debug.Assert(dbTxn is not null);

        await conn.ExecuteAsync(sqlQuery, item, dbTxn);
    }

    /// <summary>
    ///     Run a parameterized query that returns an int
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="conn"></param>
    /// <param name="sqlQuery"></param>
    /// <param name="items"></param>
    /// <param name="dbTxn"></param>
    /// <returns></returns>
    public static async Task<int> ExecuteScalarQueryAsync<T>(
        this IDbConnection conn,
        string sqlQuery,
        ImmutableArray<T> items,
        IDbTransaction dbTxn
    )
    {
        Debug.Assert(!items.IsDefaultOrEmpty);
        Debug.Assert(!string.IsNullOrWhiteSpace(sqlQuery));
        Debug.Assert(conn is not null);
        Debug.Assert(dbTxn is not null);

        return await conn.ExecuteScalarAsync<int>(sqlQuery, items, dbTxn);
    }

    /// <summary>
    ///     Run a query that returns an int
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="sqlQuery"></param>
    /// <param name="dbTxn"></param>
    /// <returns></returns>
    public static async Task<int> ExecuteScalarQueryAsync(
        this IDbConnection conn,
        string sqlQuery,
        IDbTransaction dbTxn
    )
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(sqlQuery));
        Debug.Assert(conn is not null);
        Debug.Assert(dbTxn is not null);

        return await conn.ExecuteScalarAsync<int>(sqlQuery, dbTxn);
    }
}
