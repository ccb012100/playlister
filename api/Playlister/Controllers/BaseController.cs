using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Playlister.Utilities;

namespace Playlister.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : Controller
{
    private readonly IAccessTokenUtility _accessTokenUtility;
    protected readonly IMediator Mediator;

    protected BaseController(IMediator mediator, IAccessTokenUtility accessTokenUtility)
    {
        Mediator = mediator;
        _accessTokenUtility = accessTokenUtility;
    }

    protected string CookieToken => _accessTokenUtility.GetTokenFromUserCookie();

    /// <summary>
    ///     Run a function inside a timer and return the data it produces
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <typeparam name="T">The type of data returned by <paramref name="func" /></typeparam>
    /// <returns>A <see cref="ValueTuple{T1,T2}" /> containing the data returned from <paramref name="func" /> and the time it took to run it.</returns>
    protected static async Task<(T data, TimeSpan elapsed)> RunInTimer<T>(Func<Task<T>> func)
    {
        Stopwatch sw = new();
        sw.Start();

        T result = await func();

        sw.Stop();

        return new ValueTuple<T, TimeSpan>(result, sw.Elapsed);
    }

    /// <summary>
    ///     Run a function inside a timer
    /// </summary>
    /// <param name="func">The function to run</param>
    /// <returns>The time it took to run <paramref name="func" /></returns>
    protected static async Task<TimeSpan> RunInTimer(Func<Task> func)
    {
        Stopwatch sw = new();
        sw.Start();

        await func();

        sw.Stop();

        return sw.Elapsed;
    }
}
