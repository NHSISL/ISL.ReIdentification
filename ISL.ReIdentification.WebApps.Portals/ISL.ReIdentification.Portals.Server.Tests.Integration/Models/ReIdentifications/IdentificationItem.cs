﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ReIdentifications
{
    public class IdentificationItem
    {
        public string RowNumber { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool HasAccess { get; set; } = false;
        public bool IsReidentified { get; set; } = false;
    }
}
