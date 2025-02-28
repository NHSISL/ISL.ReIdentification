using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class UserAgreementValidationException : Xeption
    {
        public UserAgreementValidationException(string message, Xeption innerException)
            : base(message,innerException)
        { }
    }
}