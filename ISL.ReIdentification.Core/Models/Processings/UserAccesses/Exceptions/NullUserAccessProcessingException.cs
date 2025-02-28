// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class NullUserAccessProcessingException : Xeption
    {
        public NullUserAccessProcessingException(string message)
            : base(message)
        { }
    }
}
