// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;

namespace ISL.ReIdentification.Core.Brokers.Hashing
{
    public interface IHashBroker
    {
        string GenerateMd5Hash(Stream data);
        string GenerateSha256Hash(Stream data);
    }
}
