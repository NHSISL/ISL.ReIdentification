﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.DelegatedAccesses
{
    public class DelegatedAccess
    {
        public Guid Id { get; set; }
        public string RequesterEmail { get; set; }
        public string RecipientEmail { get; set; }
        public bool IsDelegatedAccess { get; set; }
        public bool IsApproved { get; set; }
        public byte[]? Data { get; set; }
        public string IdentifierColumn { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}