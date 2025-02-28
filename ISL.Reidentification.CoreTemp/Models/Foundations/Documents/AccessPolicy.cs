// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents
{
    public class AccessPolicy
    {
        public string PolicyName { get; set; }
        public List<string> Permissions { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? ExpiryTime { get; set; }
    }
}
