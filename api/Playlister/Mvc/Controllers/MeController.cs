using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Commands;
using Playlister.CQRS.Handlers;
using Playlister.Utilities;
using Playlister.ViewModels;

namespace Playlister.Controllers;

[ValidateTokenCookie]
public class MeController : Controller
{
    public const string Name = "Me";

    private readonly IAccessTokenUtility _tokenUtility;
    private readonly GetCurrentUserHandler _userHandler;

    public MeController( GetCurrentUserHandler userHandler, IAccessTokenUtility tokenUtility )
    {
        _userHandler = userHandler;
        _tokenUtility = tokenUtility;
    }

    public async Task<IActionResult> Index()
    {
        return View( new MeViewModel( await _userHandler.Handle( new GetCurrentUserCommand( _tokenUtility.GetTokenFromUserCookie() ) ) ) );
    }
}
