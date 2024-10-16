// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Securities;
using Microsoft.AspNetCore.Http;

namespace ISL.ReIdentification.Core.Brokers.Securities
{
    public class SecurityBroker : ISecurityBroker
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SecurityBroker(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<bool> HasClaimType(ClaimsPrincipal user, string claimType, string claimValue)
        {
            return user.HasClaim(claimType, claimValue);
        }

        public async ValueTask<bool> HasClaimType(ClaimsPrincipal user, string claimType)
        {
            return user.FindFirst(claimType) != null;
        }

        public async ValueTask<bool> IsCurrentUserAuthenticated() =>
            this.httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public async ValueTask<bool> IsInRole(ClaimsPrincipal user, string roleName)
        {
            var roles = user.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList();

            return roles.Contains(roleName);
        }

        public async ValueTask<EntraUser> GetCurrentUser()
        {
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;

            var entraUserId = user.FindFirst("oid")?.Value
                      ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            var givenName = user.FindFirst(ClaimTypes.GivenName)?.Value;
            var surname = user.FindFirst(ClaimTypes.Surname)?.Value;
            var displayName = user.FindFirst("displayName")?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var jobTitle = user.FindFirst("jobTitle")?.Value;
            var roles = user.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList();
            var claimsList = user.Claims;

            EntraUser entraUser = new EntraUser(

                entraUserId: entraUserId,
                givenName: givenName,
                surname: surname,
                displayName: displayName,
                email: email,
                jobTitle: jobTitle,
                roles: roles,
                claims: claimsList);

            return entraUser;
        }
    }
}
