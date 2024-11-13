// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ImpersonationContexts;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ReIdentifications;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.Accesses
{
    public class AccessRequest
    {
        public IdentificationRequest IdentificationRequest { get; set; }
        public CsvIdentificationRequest CsvIdentificationRequest { get; set; }
        public ImpersonationContext ImpersonationContext { get; set; }
    }
}
