using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Moq;
using On4Net.Extensions.Identity.Firebase.Auth;
using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// Tests for <see cref="AuthorizationFilter"/> role checks.
/// </summary>
public sealed class AuthorizationFilterTests
{
    private static AuthorizationFilterContext CreateContext(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        var http = new DefaultHttpContext { User = principal };
        var actionContext = new ActionContext(http, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    [Fact]
    public async Task Empty_policies_forbids()
    {
        var filter = new AuthorizationFilter(Array.Empty<string>(), Mock.Of<IAuthorizationService>());
        var ctx = CreateContext(new Claim(ClaimTypes.Role, "Admin"));
        await filter.OnAuthorizationAsync(ctx);
        Assert.IsType<ForbidResult>(ctx.Result);
    }

    [Fact]
    public async Task Matching_role_claim_allows()
    {
        var filter = new AuthorizationFilter(new[] { "Admin" }, Mock.Of<IAuthorizationService>());
        var ctx = CreateContext(new Claim(ClaimTypes.Role, "Admin"));
        await filter.OnAuthorizationAsync(ctx);
        Assert.Null(ctx.Result);
    }

    [Fact]
    public async Task Missing_role_forbids()
    {
        var filter = new AuthorizationFilter(new[] { "Admin" }, Mock.Of<IAuthorizationService>());
        var ctx = CreateContext(new Claim(ClaimTypes.Role, "User"));
        await filter.OnAuthorizationAsync(ctx);
        Assert.IsType<ForbidResult>(ctx.Result);
    }
}
