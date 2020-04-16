using Microsoft.AspNetCore.Authorization;
namespace Rewind.APILayer2.Helpers
{
    public class MiddlewareReadAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "http://schemas.microsoft.com/identity/claims/scope";
        public string ClaimValue => "Middleware.Read";
    }
}
