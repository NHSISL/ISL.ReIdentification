// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Attrify.InvisibleApi.Models;
using Microsoft.AspNetCore.Authentication;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance
{
    public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public InvisibleApiKey InvisibleApiKey { get; set; }
    }
}
