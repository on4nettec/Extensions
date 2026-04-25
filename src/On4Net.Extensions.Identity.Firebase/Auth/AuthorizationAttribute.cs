using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace On4Net.Extensions.Identity.Firebase.Auth;

public class AuthorizationAttribute : TypeFilterAttribute
{
    public AuthorizationAttribute(params string[] policies)
        : base(typeof(AuthorizationFilter))
    {
        Arguments = new object[1] { policies };
    }
}
