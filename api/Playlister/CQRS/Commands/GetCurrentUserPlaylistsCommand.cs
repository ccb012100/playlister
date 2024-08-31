namespace Playlister.CQRS.Commands;

public record GetCurrentUserPlaylistsCommand(string AccessToken) : Command;
