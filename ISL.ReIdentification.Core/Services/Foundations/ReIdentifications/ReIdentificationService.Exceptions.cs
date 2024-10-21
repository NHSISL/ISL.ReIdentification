// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationService
    {
        private delegate ValueTask<IdentificationRequest> ReturningIdentificationRequestFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (HttpResponseUnauthorizedException httpResponseUnauthorizedException)
            {
                var failedClientReIdentificationException = new FailedClientReIdentificationException(
                        message: "Failed NECS client error occurred, please contact support.",
                        innerException: httpResponseUnauthorizedException,
                        data: httpResponseUnauthorizedException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(failedClientReIdentificationException);
            }
            catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
            {
                var failedClientReIdentificationException = new FailedClientReIdentificationException(
                    message: "Failed NECS client error occurred, please contact support.",
                    innerException: httpResponseUrlNotFoundException,
                    data: httpResponseUrlNotFoundException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(failedClientReIdentificationException);
            }
            catch (HttpResponseBadRequestException httpResponseBadRequestException)
            {
                var failedClientReIdentificationException = new FailedClientReIdentificationException(
                        message: "Failed NECS client error occurred, please contact support.",
                        innerException: httpResponseBadRequestException,
                        data: httpResponseBadRequestException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(failedClientReIdentificationException);
            }
            catch (HttpResponseInternalServerErrorException httpResponseInternalServerErrorException)
            {
                var failedServerReIdentificationException =
                    new FailedServerReIdentificationException(
                        message: "Failed NECS server error occurred, please contact support.",
                        innerException: httpResponseInternalServerErrorException,
                        data: httpResponseInternalServerErrorException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedServerReIdentificationException);
            }
            catch (Exception exception)
            {
                var failedServiceIdentificationRequestException =
                    new FailedServiceReIdentificationException(
                        message: "Failed re-identification service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationRequestException);
            }
        }

        private async ValueTask<IdentificationRequest> TryCatch(
            ReturningIdentificationRequestFunction returningIdentificationRequestFunction)
        {
            try
            {
                return await returningIdentificationRequestFunction();
            }
            catch (NullIdentificationRequestException nullIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullIdentificationRequestException);
            }
            catch (InvalidIdentificationRequestException invalidIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidIdentificationRequestException);
            }
            catch (AggregateException aggregateException)
            {
                var failedServiceIdentificationRequestException =
                    new FailedServiceReIdentificationException(
                        message: "Failed re-identification aggregate service error occurred, please contact support.",
                        innerException: aggregateException,
                        data: aggregateException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationRequestException);
            }
            catch (Exception exception)
            {
                var failedServiceIdentificationRequestException =
                    new FailedServiceReIdentificationException(
                        message: "Failed re-identification service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationRequestException);
            }
        }

        private async ValueTask<IdentificationRequestValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var accessAuditValidationException = new IdentificationRequestValidationException(
                message: "Re-identification validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessAuditValidationException);

            return accessAuditValidationException;
        }


        private async ValueTask<ReIdentificationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var reidentificationDependencyValidationException = new ReIdentificationDependencyValidationException(
                message: "Re-identification dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(reidentificationDependencyValidationException);

            return reidentificationDependencyValidationException;
        }

        private async ValueTask<ReIdentificationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var reidentificationDependencyException = new ReIdentificationDependencyException(
                message: "Re-identification dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(reidentificationDependencyException);

            return reidentificationDependencyException;
        }

        private async ValueTask<ReIdentificationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var reidentificationServiceException = new ReIdentificationServiceException(
                message: "Service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(reidentificationServiceException);

            return reidentificationServiceException;
        }
    }
}
