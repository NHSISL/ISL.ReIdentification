// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAccesses
{
    public partial class UserAccessProcessingService
    {
        private delegate ValueTask<UserAccess> ReturningUserAccessFunction();

        private async ValueTask<UserAccess> TryCatch(ReturningUserAccessFunction returningUserAccessFunction)
        {
            try
            {
                return await returningUserAccessFunction();
            }
            catch (NullUserAccessProcessingException nullUserAccessProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullUserAccessProcessingException);
            }
        }

        private async ValueTask<UserAccessProcessingValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var userAccessProcessingValidationException = new UserAccessProcessingValidationException(
                message: "User access processing validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAccessProcessingValidationException);

            return userAccessProcessingValidationException;
        }
    }
}
