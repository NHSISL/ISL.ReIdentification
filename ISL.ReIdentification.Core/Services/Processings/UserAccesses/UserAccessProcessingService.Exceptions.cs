// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
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
            catch (UserAccessValidationException userAccessValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(userAccessValidationException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(userAccessDependencyValidationException);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(userAccessDependencyException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(userAccessServiceException);
            }
            catch (Exception exception)
            {
                var failedUserAccessProcessingServiceException =
                    new FailedUserAccessProcessingServiceException(
                        message: "Failed user access processing service error occurred, please contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedUserAccessProcessingServiceException);
            }
        }

        private async ValueTask<UserAccessProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var userAccessProcessingValidationException = new UserAccessProcessingValidationException(
                message: "User access processing validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAccessProcessingValidationException);

            return userAccessProcessingValidationException;
        }

        private async ValueTask<UserAccessProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var addressProcessingDependencyValidationException =
                new UserAccessProcessingDependencyValidationException(
                    message: "User access processing dependency validation error occurred, please try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(addressProcessingDependencyValidationException);

            return addressProcessingDependencyValidationException;
        }

        private async ValueTask<UserAccessProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var addressProcessingDependencyException =
                new UserAccessProcessingDependencyException(
                    message: "User access processing dependency error occurred, please try again.",
                    innerException: exception?.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(addressProcessingDependencyException);

            throw addressProcessingDependencyException;
        }

        private async ValueTask<UserAccessProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var addressProcessingServiceException = new
                UserAccessProcessingServiceException(
                    message: "User access processing service error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(addressProcessingServiceException);

            return addressProcessingServiceException;
        }
    }
}
