// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class InvalidPdsDataException : Xeption
    {
        public InvalidPdsDataException(string message)
            : base(message)
        { }
    }
}