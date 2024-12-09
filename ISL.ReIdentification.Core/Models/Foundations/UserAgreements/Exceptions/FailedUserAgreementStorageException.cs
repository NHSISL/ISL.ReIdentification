using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class FailedUserAgreementStorageException : Xeption
    {
        public FailedUserAgreementStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}