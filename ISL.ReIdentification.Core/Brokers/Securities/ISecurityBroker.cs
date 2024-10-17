// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Security.Claims;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Securities;

namespace ISL.ReIdentification.Core.Brokers.Securities
{
    public interface ISecurityBroker
    {
        ValueTask<EntraUser> GetCurrentUser();
        ValueTask<bool> IsCurrentUserAuthenticated();
        ValueTask<bool> IsInRole(ClaimsPrincipal user, string roleName);
        ValueTask<bool> HasClaimType(ClaimsPrincipal user, string claimType, string claimValue);
        ValueTask<bool> HasClaimType(ClaimsPrincipal user, string claimType);
    }
}
