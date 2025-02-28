// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class NotFoundPdsDataException : Xeption
    {
        public NotFoundPdsDataException(string message)
            : base(message)
        { }
    }
}