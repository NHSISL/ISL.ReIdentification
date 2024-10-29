// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
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
        public async Task ShouldThrowDependencyValidationOnAddDocumentAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            string someFileName = GetRandomString();
            string someContainer = GetRandomString();
            Stream someStream = new HasLengthStream();

            this.documentServiceMock.Setup(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask addDocumentTask =
                this.identificationOrchestrationService
                    .AddDocumentAsync(someStream, someFileName, someContainer);

            IdentificationOrchestrationDependencyValidationException
                actualIdentificationOrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationDependencyValidationException>(
                        testCode: addDocumentTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyValidationException);

            this.documentServiceMock.Verify(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyValidationException))),
                       Times.Once);

            this.documentServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DocumentDependencyExceptions))]
        public async Task ShouldThrowDependencyOnAddDocumentAndLogItAsync(Xeption dependencyException)
        {
            // given
            string someFileName = GetRandomString();
            string someContainer = GetRandomString();
            Stream someStream = new HasLengthStream();

            this.documentServiceMock.Setup(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer))
                    .ThrowsAsync(dependencyException);

            var expectedIdentificationOrchestrationDependencyException =
                new IdentificationOrchestrationDependencyException(
                    message: "Identification orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask addDocumentTask =
                this.identificationOrchestrationService
                    .AddDocumentAsync(someStream, someFileName, someContainer);

            IdentificationOrchestrationDependencyException
                actualIdentificationOrchestrationDependencyException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationDependencyException>(
                        testCode: addDocumentTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyException);

            this.documentServiceMock.Verify(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyException))),
                       Times.Once);

            this.documentServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddDocumentIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            string someFileName = GetRandomString();
            string someContainer = GetRandomString();
            Stream someStream = new HasLengthStream();
            var someException = new Exception();

            var failedServiceIdentificationOrchestrationException =
                new FailedServiceIdentificationOrchestrationException(
                    message: "Failed service identification orchestration error occurred, contact support.",
                    innerException: someException);

            var expectedIdentificationOrchestrationServiceException =
                new IdentificationOrchestrationServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceIdentificationOrchestrationException);

            this.documentServiceMock.Setup(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer))
                    .ThrowsAsync(someException);

            // when
            ValueTask addDocumentTask =
                this.identificationOrchestrationService
                    .AddDocumentAsync(someStream, someFileName, someContainer);

            IdentificationOrchestrationServiceException
                actualIdentificationOrchestrationValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationServiceException>(
                        testCode: addDocumentTask.AsTask);

            // then
            actualIdentificationOrchestrationValidationException.Should().BeEquivalentTo(
                expectedIdentificationOrchestrationServiceException);

            this.documentServiceMock.Verify(service =>
                service.AddDocumentAsync(someStream, someFileName, someContainer),
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
