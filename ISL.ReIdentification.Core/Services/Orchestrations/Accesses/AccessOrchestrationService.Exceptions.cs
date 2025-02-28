// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationService
    {
        private delegate ValueTask<List<string>> ReturningOrganisationsFunction();
        private delegate ValueTask<bool> ReturningBooleanFunction();
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
            catch (InvalidArgumentAccessOrchestrationException invalidArgumentAccessOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentAccessOrchestrationException);
            }
            catch (UnauthorizedAccessOrchestrationException unauthorizedAccessOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorizedAccessOrchestrationException);
            }
            catch (UserAccessValidationException userAccessValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    userAccessValidationException);
            }
            catch (UserAccessDependencyValidationException userAccessDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    userAccessDependencyValidationException);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    pdsDataValidationException);
            }
            catch (UserAccessServiceException userAccessServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    userAccessServiceException);
            }
            catch (UserAccessDependencyException userAccessDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    userAccessDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    pdsDataServiceException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    pdsDataDependencyException);
            }
            catch (AggregateException aggregateException)
            {
                var failedAccessOrchestrationServiceException =
                    new FailedServiceAccessOrchestrationException(
                        message: "Failed access aggregate orchestration service error occurred, " +
                            "please contact support.",
                        innerException: aggregateException);

                throw await CreateAndLogServiceExceptionAsync(failedAccessOrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceAccessOrchestrationException =
                    new FailedServiceAccessOrchestrationException(
                        message: "Failed access orchestration service error occurred, please contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceAccessOrchestrationException);
            }
        }

        private async ValueTask<AccessOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var accessOrchestrationServiceException = new AccessOrchestrationServiceException(
                message: "Access orchestration service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationServiceException);

            return accessOrchestrationServiceException;
        }

        private async ValueTask<AccessOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var accessOrchestrationValidationException =
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationValidationException);

            return accessOrchestrationValidationException;
        }

        private async ValueTask<AccessOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var accessOrchestrationDependencyValidationException =
                new AccessOrchestrationDependencyValidationException(
                    message: "Access orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationDependencyValidationException);

            return accessOrchestrationDependencyValidationException;
        }

        private async ValueTask<AccessOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var accessOrchestrationDependencyException =
                new AccessOrchestrationDependencyException(
                    message: "Access orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(accessOrchestrationDependencyException);

            return accessOrchestrationDependencyException;
        }
    }
}
