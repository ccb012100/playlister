namespace Playlister.Models;

public class DbRecordNotFoundException : Exception {
    public DbRecordNotFoundException( string message ) : base( message ) {
    }

    public DbRecordNotFoundException( string message , Exception inner ) : base( message , inner ) {
    }
}
