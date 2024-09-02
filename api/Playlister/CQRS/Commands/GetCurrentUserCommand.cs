namespace Playlister.CQRS.Commands;

public record GetCurrentUserCommand( string AccessToken ) : Command;
