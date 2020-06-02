using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SkylineWebApi.Models;

namespace SkylineWebApi.Helpers
{
    public class GroupRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; } = "groups";
        public string[] ClaimValues { get; private set; }

        public GroupRequirement(AuthorizationRule options)
        {
            ClaimValues = options.Group.Split(";").Where(c => !string.IsNullOrEmpty(c)).ToArray();
        }
    }
}
