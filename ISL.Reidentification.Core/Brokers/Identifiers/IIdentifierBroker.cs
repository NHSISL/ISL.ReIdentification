// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace ISL.Reidentification.Core.Brokers.Identifiers
{
    public interface IIdentifierBroker
    {
        ValueTask<Guid> GetIdentifierAsync();
    }
}
