﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ReIdentifications;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses
{
    public class AccessRequest
    {
        public IdentificationRequest IdentificationRequest { get; set; }
        public CsvIdentificationRequest CsvIdentificationRequest { get; set; }
        public ImpersonationContext ImpersonationContext { get; set; }
    }
}
