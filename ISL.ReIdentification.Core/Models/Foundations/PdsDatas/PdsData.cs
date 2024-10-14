// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas
{
    public class PdsData
    {
        public long RowId { get; set; }
        public string? PseudoNhsNumber { get; set; }
        public string? PrimaryCareProvider { get; set; }
        public DateTimeOffset? PrimaryCareProviderBusinessEffectiveFromDate { get; set; }
        public DateTimeOffset? PrimaryCareProviderBusinessEffectiveToDate { get; set; }
        public string? CcgOfRegistration { get; set; }
        public string? CurrentCcgOfRegistration { get; set; }
        public string? IcbOfRegistration { get; set; }
        public string? CurrentIcbOfRegistration { get; set; }
    }
}
