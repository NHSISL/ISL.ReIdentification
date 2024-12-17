// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public partial class DocumentService : IDocumentService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<AccessPolicy> ReturningAccessPolicyFunction();
        private delegate ValueTask<List<string>> ReturningStringListFunction();
        private delegate ValueTask<List<AccessPolicy>> ReturningAccessPolicyListFunction();
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
            catch (StorageProviderValidationException storageProviderValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(storageProviderValidationException);
            }
            catch (StorageProviderDependencyException storageProviderDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderDependencyException);
            }
            catch (StorageProviderServiceException storageProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceDocumentException =
                    new FailedServiceDocumentException(
                        message: "Failed service document error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceDocumentException);
            }
        }

        private async ValueTask<List<string>> TryCatch(ReturningStringListFunction returningStringListFunction)
        {
            try
            {
                return await returningStringListFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
            catch (StorageProviderValidationException storageProviderValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(storageProviderValidationException);
            }
            catch (StorageProviderDependencyException storageProviderDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderDependencyException);
            }
            catch (StorageProviderServiceException storageProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceDocumentException =
                    new FailedServiceDocumentException(
                        message: "Failed service document error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceDocumentException);
            }
        }

        private async ValueTask<List<AccessPolicy>>
            TryCatch(ReturningAccessPolicyListFunction returningAccessPolicyListFunction)
        {
            try
            {
                return await returningAccessPolicyListFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
            catch (StorageProviderValidationException storageProviderValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(storageProviderValidationException);
            }
            catch (StorageProviderDependencyException storageProviderDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderDependencyException);
            }
            catch (StorageProviderServiceException storageProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceDocumentException =
                    new FailedServiceDocumentException(
                        message: "Failed service document error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceDocumentException);
            }
        }

        private async ValueTask<AccessPolicy> TryCatch(ReturningAccessPolicyFunction returningAccessPolicyFunction)
        {
            try
            {
                return await returningPolicyFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
            catch (AccessPolicyNotFoundDocumentException accessPolicyNotFoundDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(accessPolicyNotFoundDocumentException);
            }
            catch (StorageProviderValidationException storageProviderValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(storageProviderValidationException);
            }
            catch (StorageProviderDependencyException storageProviderDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderDependencyException);
            }
            catch (StorageProviderServiceException storageProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceDocumentException =
                    new FailedServiceDocumentException(
                        message: "Failed service document error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceDocumentException);
            }
        }

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (InvalidDocumentException invalidDocumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDocumentException);
            }
            catch (StorageProviderValidationException storageProviderValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(storageProviderValidationException);
            }
            catch (StorageProviderDependencyException storageProviderDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderDependencyException);
            }
            catch (StorageProviderServiceException storageProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(storageProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceDocumentException =
                    new FailedServiceDocumentException(
                        message: "Failed service document error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceDocumentException);
            }
        }

        private async ValueTask<DocumentValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var documentValidationException =
                new DocumentValidationException(
                    message: "Document validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(documentValidationException);

            return documentValidationException;
        }

        private async ValueTask<DocumentDependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var documentDependencyValidationException =
                new DocumentDependencyValidationException(
                    message: "Document dependency validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(documentDependencyValidationException);

            return documentDependencyValidationException;
        }

        private async ValueTask<DocumentDependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var documentDependencyException =
                new DocumentDependencyException(
                    message: "Document dependency error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(documentDependencyException);

            return documentDependencyException;
        }

        private async ValueTask<DocumentServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var documentServiceException =
                new DocumentServiceException(
                    message: "Document service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(documentServiceException);

            return documentServiceException;
        }
    }
}
