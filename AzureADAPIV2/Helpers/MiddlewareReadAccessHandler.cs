using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace AzureADAPIV2.Helpers
{
    public class MiddlewareReadAccessHandler : AuthorizationHandler<MiddlewareReadAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MiddlewareReadAccessRequirement requirement)
        {
            bool hasClaim = context.User.HasClaim(c => c.Type == requirement.ClaimType);
            if (hasClaim)
            {
                string value = context.User.FindFirstValue(requirement.ClaimType);
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
