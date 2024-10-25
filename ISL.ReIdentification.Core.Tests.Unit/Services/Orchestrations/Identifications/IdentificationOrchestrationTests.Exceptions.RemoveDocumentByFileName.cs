// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(DocumentDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveDocumentByFileNameAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someFilename = GetRandomString();
            string someContainer = GetRandomString();

            this.documentServiceMock.Setup(serivce =>
                serivce.RemoveDocumentByFileNameAsync(someFilename, someContainer))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask removeDocumentByFileNamTask =
                this.identificationOrchestrationService
                    .RemoveDocumentByFileNameAsync(someFilename, someContainer);

            IdentificationOrchestrationDependencyValidationException
                actualIdentificationOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationDependencyValidationException>(
                        testCode: removeDocumentByFileNamTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyValidationException);

            this.documentServiceMock.Verify(serivce =>
                serivce.RemoveDocumentByFileNameAsync(someFilename, someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyValidationException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveDocumentByFileNameIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string someFilename = GetRandomString();
            string someContainer = GetRandomString();
            var someException = new Exception();

            var failedServiceIdentificationOrchestrationException =
                new FailedServiceIdentificationOrchestrationException(
                    message: "Failed service identification orchestration error occurred, contact support.",
                    innerException: someException);

            var expectedIdentificationOrchestrationServiceException =
                new IdentificationOrchestrationServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceIdentificationOrchestrationException);

            this.documentServiceMock.Setup(serivce =>
                serivce.RemoveDocumentByFileNameAsync(someFilename, someContainer))
                    .ThrowsAsync(someException);

            // when
            ValueTask removeDocumentByFileNamTask =
                this.identificationOrchestrationService
                    .RemoveDocumentByFileNameAsync(someFilename, someContainer);

            IdentificationOrchestrationServiceException
                actualIdentificationOrchestrationValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationServiceException>(
                        testCode: removeDocumentByFileNamTask.AsTask);

            // then
            actualIdentificationOrchestrationValidationException.Should().BeEquivalentTo(
                expectedIdentificationOrchestrationServiceException);

            this.documentServiceMock.Verify(serivce =>
                serivce.RemoveDocumentByFileNameAsync(someFilename, someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationServiceException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
