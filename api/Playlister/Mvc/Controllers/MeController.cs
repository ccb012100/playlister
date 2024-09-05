using Microsoft.AspNetCore.Mvc;
using Playlister.Attributes;
using Playlister.CQRS.Handlers;
using Playlister.CQRS.Queries;
using Playlister.Mvc.ViewModels;
using Playlister.Utilities;

namespace Playlister.Mvc.Controllers;

[ValidateTokenCookie]
public class MeController : Controller
{
    public const string Name = "Me";

    private readonly IAccessTokenUtility _tokenUtility;
    private readonly CurrentUserHandler _userHandler;

    public MeController( CurrentUserHandler userHandler, IAccessTokenUtility tokenUtility )
    {
        _userHandler = userHandler;
        _tokenUtility = tokenUtility;
    }

    [ProducesResponseType<ViewResult>( StatusCodes.Status200OK )]
    public async Task<IActionResult> Index()
    {
        return View( new MeViewModel( await _userHandler.Get( new GetCurrentUserQuery( _tokenUtility.GetTokenFromUserCookie() ) ) ) );
    }
}
