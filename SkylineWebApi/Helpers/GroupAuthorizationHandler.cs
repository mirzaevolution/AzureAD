using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace SkylineWebApi.Helpers
{
    public class GroupAuthorizationHandler : AuthorizationHandler<GroupRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            GroupRequirement requirement)
        {
            int limit = requirement.ClaimValues.Length;
            int counter = 0;
            var claims = context.User.Claims.Where(c => c.Type == requirement.ClaimType);
            if (claims != null)
            {
                foreach (var item in requirement.ClaimValues)
                {
                    if (claims.Any(c => c.Value == item))
                    {
                        counter++;
                    }
                }
                if (counter == limit)
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
