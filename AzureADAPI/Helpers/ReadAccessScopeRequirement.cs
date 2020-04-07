using Microsoft.AspNetCore.Authorization;

namespace AzureADAPI.Helpers
{
    public class ReadAccessScopeRequirement : IAuthorizationRequirement
    {
        public string ReadAccessClaimType { get; } = "http://schemas.microsoft.com/identity/claims/scope";
        public string ReadAccessClaimValue { get; } = "Access.Read";
        public ReadAccessScopeRequirement()
        {
        }
    }
}
