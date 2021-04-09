using System;
using Microsoft.AspNetCore.Authorization;

namespace Vega
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; set; }
        public string Scope { get; set; }

        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentException(null, nameof(scope));
            Issuer = issuer ?? throw new ArgumentException(null, nameof(issuer));
        }
    }
}