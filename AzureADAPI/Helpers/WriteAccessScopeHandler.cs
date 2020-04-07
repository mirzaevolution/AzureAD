using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace AzureADAPI.Helpers
{
    public class WriteAccessScopeHandler : AuthorizationHandler<WriteAccessScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WriteAccessScopeRequirement requirement)
        {
            bool claimExists = context.User.HasClaim(c => c.Type.Equals(requirement.WriteAccessClaimType));

            if (claimExists)
            {
                var claims = context.User.FindFirstValue(requirement.WriteAccessClaimType).Split(" ");
                if (claims.Contains(requirement.WriteAccessClaimValue))
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
