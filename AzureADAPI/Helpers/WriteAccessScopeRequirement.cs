using Microsoft.AspNetCore.Authorization;
namespace AzureADAPI.Helpers
{
    public class WriteAccessScopeRequirement : IAuthorizationRequirement
    {

        public string WriteAccessClaimType { get; } = "http://schemas.microsoft.com/identity/claims/scope";
        public string WriteAccessClaimValue { get; } = "Access.Write";
        public WriteAccessScopeRequirement()
        {
        }
    }
}
