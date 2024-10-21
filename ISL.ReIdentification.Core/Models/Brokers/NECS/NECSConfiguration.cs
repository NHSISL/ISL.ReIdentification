// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Core.Models.Brokers.NECS
{
    public class NECSConfiguration
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int ApiMaxBatchSize { get; set; }
    }
}
