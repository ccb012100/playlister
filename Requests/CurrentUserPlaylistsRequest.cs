using System.Collections.Generic;
using MediatR;
using Playlister.Models;

// ReSharper disable UnusedMember.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Playlister.Requests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record CurrentUserPlaylistsRequest : IRequest<IEnumerable<Playlist>>;
}
