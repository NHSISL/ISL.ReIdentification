// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;

namespace ISL.ReIdentification.Core.Models.Securities
{
    public class EntraUser
    {
        public string EntraUserId { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
