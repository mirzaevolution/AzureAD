using Microsoft.AspNetCore.Authorization;

namespace AzureADAPIV2.Helpers
{
    public class MiddlewareReadAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "http://schemas.microsoft.com/identity/claims/scope";
        public string ClaimValue => "Middleware.Read";
        public MiddlewareReadAccessRequirement()
        {
        }
    }
}
