using Playlister.Repositories;

namespace Playlister.Services.Implementations;

public class SqliteDatabaseService( ISqliteDatabaseRepository repository ) : ISqliteDatabaseService {
    private readonly ISqliteDatabaseRepository _repository = repository;

    public async Task VacuumDatabase( CancellationToken ct ) {
        await _repository.VacuumDatabase( ct );
    }
}
