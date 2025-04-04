﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.UserAccesses
{
    public class UserAccess
    {
        public Guid Id { get; set; }
        public string EntraUserId { get; set; }
        public string GivenName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string OrgCode { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
