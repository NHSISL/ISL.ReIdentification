﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses
{
    public class UserAccess : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string EntraUserId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string UserPrincipalName { get; set; }
        public string OrgCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
