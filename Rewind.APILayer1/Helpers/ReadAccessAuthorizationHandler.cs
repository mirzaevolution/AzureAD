using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Rewind.APILayer1.Helpers
{
    public class ReadAccessAuthorizationHandler : AuthorizationHandler<ReadAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadAccessRequirement requirement)
        {
            bool hasClaim = context.User.HasClaim(c => c.Type == requirement.ClaimType);
            if (hasClaim)
            {
                string[] arrayClaims = context.User.FindFirstValue(requirement.ClaimType)?.Split(" ");
                if (arrayClaims != null && arrayClaims.Contains(requirement.ClaimValue))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
