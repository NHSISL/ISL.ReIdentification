// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService
    {
        private delegate ValueTask<AccessRequest> ReturningAccessRequestFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<AccessRequest> TryCatch(ReturningAccessRequestFunction returningAccessRequestFunction)
        {
            try
            {
                return await returningAccessRequestFunction();
            }
            catch (NullAccessRequestException nullAccessRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullAccessRequestException);
            }
            catch (InvalidArgumentPersistanceOrchestrationException invalidArgumentPersistanceOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentPersistanceOrchestrationException);
            }
            catch (ImpersonationContextValidationException impersonationContextValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    impersonationContextValidationException);
            }
            catch (ImpersonationContextDependencyValidationException impersonationContextDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    impersonationContextDependencyValidationException);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    csvIdentificationRequestValidationException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    csvIdentificationRequestDependencyValidationException);
            }
            catch (NotificationValidationException notificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationValidationException);
            }
            catch (NotificationDependencyValidationException notificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationDependencyValidationException);
            }
            catch (ImpersonationContextDependencyException impersonationContextDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    impersonationContextDependencyException);
            }
            catch (ImpersonationContextServiceException impersonationContextServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    impersonationContextServiceException);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    csvIdentificationRequestServiceException);
            }
            catch (NotificationDependencyException notificationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationDependencyException);
            }
            catch (NotificationServiceException notificationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationServiceException);
            }
            catch (Exception exception)
            {
                var failedServicePersistanceOrchestrationException =
                    new FailedServicePersistanceOrchestrationException(
                        message: "Failed service persistance orchestration error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServicePersistanceOrchestrationException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullCsvReIdentificationConfigurationException nullCsvReIdentificationConfigurationException)
            {
                throw await CreateAndLogCriticalValidationExceptionAsync(nullCsvReIdentificationConfigurationException);
            }
            catch (NullAccessRequestException nullAccessRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullAccessRequestException);
            }
            catch (InvalidArgumentPersistanceOrchestrationException invalidArgumentPersistanceOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentPersistanceOrchestrationException);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    csvIdentificationRequestValidationException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    csvIdentificationRequestDependencyValidationException);
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
            catch (NotificationValidationException notificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationValidationException);
            }
            catch (NotificationDependencyValidationException notificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    notificationDependencyValidationException);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    csvIdentificationRequestServiceException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    accessAuditServiceException);
            }
            catch (NotificationDependencyException notificationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationDependencyException);
            }
            catch (NotificationServiceException notificationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    notificationServiceException);
            }
            catch (Exception exception)
            {
                var failedServicePersistanceOrchestrationException =
                    new FailedServicePersistanceOrchestrationException(
                        message: "Failed service persistance orchestration error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServicePersistanceOrchestrationException);
            }
        }

        private async ValueTask<PersistanceOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var persistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(persistanceOrchestrationValidationException);

            return persistanceOrchestrationValidationException;
        }

        private async ValueTask<PersistanceOrchestrationValidationException>
            CreateAndLogCriticalValidationExceptionAsync(Xeption exception)
        {
            var persistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(persistanceOrchestrationValidationException);

            return persistanceOrchestrationValidationException;
        }

        private async ValueTask<PersistanceOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var persistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(persistanceOrchestrationDependencyValidationException);

            return persistanceOrchestrationDependencyValidationException;
        }

        private async ValueTask<PersistanceOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var persistanceOrchestrationDependencyException =
                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(persistanceOrchestrationDependencyException);

            return persistanceOrchestrationDependencyException;
        }

        private async ValueTask<PersistanceOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var persistanceOrchestrationServiceException = new PersistanceOrchestrationServiceException(
                message: "Persistance orchestration service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(persistanceOrchestrationServiceException);

            return persistanceOrchestrationServiceException;
        }
    }
}
