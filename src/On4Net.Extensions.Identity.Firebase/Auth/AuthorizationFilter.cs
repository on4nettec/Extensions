using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace On4Net.Extensions.Identity.Firebase.Auth;

public class AuthorizationFilter : IAsyncAuthorizationFilter, IFilterMetadata
{
    private readonly IAuthorizationService _authorizationService;

    public string[] Policies { get; set; }

    public AuthorizationFilter(string[] policies, IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
        Policies = policies;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (Policies == null || Policies.Length == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        string[] policies = Policies;
        foreach (var identity in context.HttpContext.User.Identities)
        {
            var claim = identity.Claims.Where(fn => fn.Type == ClaimTypes.Role);
            foreach (string policyName in policies)
            {
                if (claim.Any(fn => fn.Value == policyName))
                {
                    return;
                }
            }

        }


        context.Result = new ForbidResult();
    }
}