// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions
{
    public class NullPersistanceRequestException : Xeption
    {
        public NullPersistanceRequestException(string message)
            : base(message)
        { }
    }
}
