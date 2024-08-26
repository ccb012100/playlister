using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : Controller
    {
        private readonly IAccessTokenUtility? _accessTokenUtility;
        internal readonly IMediator _mediator;

        protected BaseController(IMediator mediator, IAccessTokenUtility accessTokenUtility)
        {
            _mediator = mediator;
            _accessTokenUtility = accessTokenUtility;
        }

        protected BaseController(IMediator mediator) => _mediator = mediator;

        internal string AccessToken =>
            _accessTokenUtility?.GetAccessTokenFromCurrentHttpContext()
            ?? throw new InvalidOperationException(
                "This controller does not have an IAccessTokenUtility"
            );
    }
}
