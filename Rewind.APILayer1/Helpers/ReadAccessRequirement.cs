using Microsoft.AspNetCore.Authorization;
namespace Rewind.APILayer1.Helpers
{
    public class ReadAccessRequirement : IAuthorizationRequirement
    {
        public string ClaimType => "http://schemas.microsoft.com/identity/claims/scope";
        public string ClaimValue => "Access.Read";
        public ReadAccessRequirement() { }
    }
}
