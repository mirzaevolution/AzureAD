using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace SkylineWebApi.Helpers
{
    public class CountryAuthorizationHandler : AuthorizationHandler<CountryRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CountryRequirement requirement)
        {
            var claim = context.User.Claims.FirstOrDefault(c => c.Type.Equals(requirement.ClaimType));
            if (claim != null)
            {
                if (claim.Value.Equals(requirement.ClaimValue))
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
