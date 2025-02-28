// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions
{
    public class NullAccessRequestException : Xeption
    {
        public NullAccessRequestException(string message)
            : base(message)
        { }
    }
}
