// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions
{
    public class FailedOperationImpersonationContextException : Xeption
    {
        public FailedOperationImpersonationContextException(string message, DbUpdateException innerException)
            : base(message, innerException)
        { }
    }
}
