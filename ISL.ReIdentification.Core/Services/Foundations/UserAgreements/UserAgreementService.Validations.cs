using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService
    {
        private void ValidateUserAgreementOnAdd(UserAgreement userAgreement)
        {
            ValidateUserAgreementIsNotNull(userAgreement);
        }

        private static void ValidateUserAgreementIsNotNull(UserAgreement userAgreement)
        {
            if (userAgreement is null)
            {
                throw new NullUserAgreementException(message: "UserAgreement is null.");
            }
        }
    }
}