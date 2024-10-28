// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationService : IIdentificationOrchestrationService
    {
        private delegate ValueTask<IdentificationRequest> ReturningIdentificationRequestFunction();
        private delegate ValueTask ReturningNothingFunction();

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
            catch (AccessAuditValidationException accessAuditValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    accessAuditValidationException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    accessAuditDependencyValidationException);
            }
            catch (ReIdentificationValidationException reIdentificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    reIdentificationValidationException);
            }
            catch (ReIdentificationDependencyValidationException reIdentificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    reIdentificationDependencyValidationException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessAuditServiceException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessAuditDependencyException);
            }
            catch (ReIdentificationServiceException reIdentificationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    reIdentificationServiceException);
            }
            catch (ReIdentificationDependencyException reIdentificationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    reIdentificationDependencyException);
            }
            catch (Exception exception)
            {
                var failedServiceIdentificationOrchestrationException =
                    new FailedServiceIdentificationOrchestrationException(
                        message: "Failed service identification orchestration error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationOrchestrationException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidArgumentIdentificationOrchestrationException
                invalidArgumentIdentificationOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentIdentificationOrchestrationException);
            }
            catch (DocumentValidationException documentValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    documentValidationException);
            }
            catch (DocumentDependencyValidationException documentDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    documentDependencyValidationException);
            }
            catch (DocumentDependencyException documentDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    documentDependencyException);
            }
            catch (DocumentServiceException documentServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    documentServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceIdentificationOrchestrationException =
                    new FailedServiceIdentificationOrchestrationException(
                        message: "Failed service identification orchestration error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceIdentificationOrchestrationException);
            }
        }

        private async ValueTask<IdentificationOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var identificationOrchestrationValidationException =
                new IdentificationOrchestrationValidationException(
                    message: "Identification orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(identificationOrchestrationValidationException);

            return identificationOrchestrationValidationException;
        }

        private async ValueTask<IdentificationOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var identificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(identificationOrchestrationDependencyValidationException);

            return identificationOrchestrationDependencyValidationException;
        }

        private async ValueTask<IdentificationOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var identificationOrchestrationDependencyException =
                new IdentificationOrchestrationDependencyException(
                    message: "Identification orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(identificationOrchestrationDependencyException);

            return identificationOrchestrationDependencyException;
        }

        private async ValueTask<IdentificationOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var identificationOrchestrationServiceException = new IdentificationOrchestrationServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(identificationOrchestrationServiceException);

            return identificationOrchestrationServiceException;
        }
    }
}
