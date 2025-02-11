﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ReIdentifications
{
    public class IdentificationRequest
    {
        public Guid Id { get; set; }
        public List<IdentificationItem> IdentificationItems { get; set; }
        public string EntraUserId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Organisation { get; set; }
        public string Reason { get; set; }
    }
}
