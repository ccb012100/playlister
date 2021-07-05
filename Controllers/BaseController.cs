using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Utilities;

namespace Playlister.Controllers
{
    [ApiController, Route("api/[controller]")]
    public abstract class BaseController : Controller
    {
        internal readonly IMediator Mediator;
        private readonly IAccessTokenUtility? _accessTokenUtility;

        internal string AccessToken =>
            _accessTokenUtility?.GetAccessTokenFromCurrentHttpContext()
            ?? throw new InvalidOperationException("This controller does not have an IAccessTokenUtility.");

        protected BaseController(IMediator mediator, IAccessTokenUtility accessTokenUtility)
        {
            Mediator = mediator;
            _accessTokenUtility = accessTokenUtility;
        }

        protected BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
