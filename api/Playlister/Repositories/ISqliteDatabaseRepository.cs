namespace Playlister.Repositories;

/// <summary>
///     Repository for running meta-queries operating on a **SQLite** database.
/// </summary>
public interface ISqliteDatabaseRepository {
    /// <summary>
    ///     Run the <c>VACUUM</c> command on the database.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task VacuumDatabase( CancellationToken ct );
}