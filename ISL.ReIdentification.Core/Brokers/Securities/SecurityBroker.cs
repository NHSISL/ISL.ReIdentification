// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Securities;
using Microsoft.AspNetCore.Http;

namespace ISL.ReIdentification.Core.Brokers.Securities
{
    /// <summary>
    /// Provides security-related functionalities such as user authentication, claim verification, and role checks.
    /// Supports both REST API (using <see cref="IHttpContextAccessor"/>) and Azure Functions (using access token).
    /// </summary>
    public class SecurityBroker : ISecurityBroker
    {
        private readonly ClaimsPrincipal user;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityBroker"/> class using <see cref="IHttpContextAccessor"/>.
        /// This constructor is intended for REST API usage.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        public SecurityBroker(IHttpContextAccessor httpContextAccessor) =>
            this.user = httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityBroker"/> class using an access token.
        /// This constructor is intended for Azure Function / non REST API usage.
        /// </summary>
        /// <param name="accessToken">A JWT access token containing user claims.</param>
        public SecurityBroker(string accessToken) =>
            this.user = GetClaimsPrincipalFromToken(accessToken);

        /// <summary>
        /// Checks whether the current user has a specific claim with a given value.
        /// </summary>
        /// <param name="claimType">The type of the claim.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <returns>True if the user has the claim with the specified value; otherwise, false.</returns>
        public async ValueTask<bool> HasClaimTypeAsync(string claimType, string claimValue) =>
            this.user.HasClaim(claimType, claimValue);

        /// <summary>
        /// Checks whether the current user has a specific claim type.
        /// </summary>
        /// <param name="claimType">The type of the claim.</param>
        /// <returns>True if the user has the claim; otherwise, false.</returns>
        public async ValueTask<bool> HasClaimTypeAsync(string claimType) =>
            this.user.FindFirst(claimType) != null;

        /// <summary>
        /// Determines whether the current user is authenticated.
        /// </summary>
        /// <returns>True if the user is authenticated; otherwise, false.</returns>
        public async ValueTask<bool> IsCurrentUserAuthenticatedAsync() =>
            this.user.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Checks if the current user is in a specified role.
        /// </summary>
        /// <param name="roleName">The role name to check.</param>
        /// <returns>True if the user is in the specified role; otherwise, false.</returns>
        public async ValueTask<bool> IsInRoleAsync(string roleName)
        {
            var roles = this.user.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList();
            return roles.Contains(roleName);
        }

        /// <summary>
        /// Retrieves details of the current authenticated user based on claims.
        /// </summary>
        /// <returns>An <see cref="EntraUser"/> object containing user details.</returns>
        public async ValueTask<EntraUser> GetCurrentUserAsync()
        {
            var entraUserIdString = this.user.FindFirst("oid")?.Value
                          ?? this.user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            var entraUserId = Guid.TryParse(entraUserIdString, out var parsedGuid) ? parsedGuid : Guid.Empty;

            var givenName = this.user.FindFirst(ClaimTypes.GivenName)?.Value;
            var surname = this.user.FindFirst(ClaimTypes.Surname)?.Value;
            var displayName = this.user.FindFirst("displayName")?.Value;
            var email = this.user.FindFirst(ClaimTypes.Email)?.Value;
            var jobTitle = this.user.FindFirst("jobTitle")?.Value;
            var roles = this.user.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList();
            var claimsList = this.user.Claims;

            return new EntraUser(
                entraUserId: entraUserId,
                givenName: givenName,
                surname: surname,
                displayName: displayName,
                email: email,
                jobTitle: jobTitle,
                roles: roles,
                claims: claimsList);
        }

        /// <summary>
        /// Extracts a <see cref="ClaimsPrincipal"/> from a given JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> containing claims from the token.</returns>
        private static ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
    }
}
