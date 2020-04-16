using Microsoft.AspNetCore.Authorization;

namespace Rewind.APILayer1.Helpers
{
    public class WriteAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "http://schemas.microsoft.com/identity/claims/scope";
        public string ClaimValue => "Access.Write";
        public WriteAccessRequirement() { }
    }
}
