using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace AzureADAPI.Helpers
{
    public class ReadAcccessScopeHandler : AuthorizationHandler<ReadAccessScopeRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadAccessScopeRequirement requirement)
        {

            bool claimExists = context.User.HasClaim(c => c.Type.Equals(requirement.ReadAccessClaimType));
            if (claimExists)
            {
                var claims = context.User.FindFirstValue(requirement.ReadAccessClaimType).Split(" ");
                if (claims.Contains(requirement.ReadAccessClaimValue))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
