using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : Controller
{
    private readonly IAccessTokenUtility _accessTokenUtility;
    internal readonly IMediator _mediator;

    protected BaseController(IMediator mediator, IAccessTokenUtility accessTokenUtility)
    {
        _mediator = mediator;
        _accessTokenUtility = accessTokenUtility;
    }

    internal string AuthHeaderAccessToken => _accessTokenUtility.GetAccessTokenFromRequestAuthHeader();

    internal string CookieAccessToken => _accessTokenUtility.GetTokenFromUserCookie();
}
