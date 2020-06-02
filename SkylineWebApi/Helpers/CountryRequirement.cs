using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SkylineWebApi.Models;

namespace SkylineWebApi.Helpers
{
    public class CountryRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; } = "ctry";
        public string ClaimValue { get; private set; } = "ID";
        public CountryRequirement(AuthorizationRule options)
        {
            if (options != null && !string.IsNullOrEmpty(options.Country))
            {
                ClaimValue = options.Country;
            }
        }
    }
}
