using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rewind.APILayer2.Helpers
{
    public class MiddlewareReadAccessAuthorizationHandler : AuthorizationHandler<MiddlewareReadAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MiddlewareReadAccessRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == requirement.ClaimType))
            {
                var value = context.User.FindFirstValue(requirement.ClaimType);
                if (value.Equals(requirement.ClaimValue))
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
