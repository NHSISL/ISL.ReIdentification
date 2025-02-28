using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class InvalidUserAgreementException : Xeption
    {
        public InvalidUserAgreementException(string message)
            : base(message)
        { }
    }
}