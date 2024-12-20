// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(DocumentDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnExpireRenewImpersonationContextTokensAndLogItAsync(
                    Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            bool someIsPreviouslyApproved = true;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationOrchestrationDependencyValidationException =
                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> expireRenewImpersonationContextTokensTask =
                this.identificationOrchestrationService
                    .ExpireRenewImpersonationContextTokensAsync(
                        someAccessRequest,
                        someIsPreviouslyApproved);

            IdentificationOrchestrationDependencyValidationException
                actualIdentificationOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<IdentificationOrchestrationDependencyValidationException>(
                    testCode: expireRenewImpersonationContextTokensTask.AsTask);

            // then
            actualIdentificationOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationDependencyValidationException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        //[Theory]
        //[MemberData(nameof(DependencyExceptions))]
        //public async Task ShouldThrowDependencyOnExpireRenewImpersonationContextTokensAndLogItAsync(
        //    Xeption dependencyValidationException)
        //{
        //    // given
        //    Guid someId = Guid.NewGuid();

        //    this.impersonationContextServiceMock.Setup(service =>
        //        service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
        //            .ThrowsAsync(dependencyValidationException);

        //    var expectedIdentificationOrchestrationDependencyException =
        //        new IdentificationOrchestrationDependencyException(
        //            message: "Identification orchestration dependency error occurred, " +
        //                "fix the errors and try again.",
        //            innerException: dependencyValidationException.InnerException as Xeption);

        //    // when
        //    ValueTask<AccessRequest> expireRenewImpersonationContextTokensTask =
        //        this.identificationOrchestrationService
        //            .ExpireRenewImpersonationContextTokensAsync(someId);

        //    IdentificationOrchestrationDependencyException
        //        actualIdentificationOrchestrationDependencyException =
        //        await Assert.ThrowsAsync<IdentificationOrchestrationDependencyException>(
        //            testCode: expireRenewImpersonationContextTokensTask.AsTask);

        //    // then
        //    actualIdentificationOrchestrationDependencyException
        //        .Should().BeEquivalentTo(expectedIdentificationOrchestrationDependencyException);

        //    this.impersonationContextServiceMock.Verify(service =>
        //        service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
        //            Times.Once);

        //    this.loggingBrokerMock.Verify(broker =>
        //       broker.LogErrorAsync(It.Is(SameExceptionAs(
        //           expectedIdentificationOrchestrationDependencyException))),
        //               Times.Once);

        //    this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        //    this.impersonationContextServiceMock.VerifyNoOtherCalls();
        //    this.notificationServiceMock.VerifyNoOtherCalls();
        //    this.accessAuditServiceMock.VerifyNoOtherCalls();
        //    this.documentServiceMock.VerifyNoOtherCalls();
        //    this.loggingBrokerMock.VerifyNoOtherCalls();
        //    this.hashBrokerMock.VerifyNoOtherCalls();
        //    this.dateTimeBrokerMock.VerifyNoOtherCalls();
        //    this.identifierBrokerMock.VerifyNoOtherCalls();
        //}

        //[Fact]
        //public async Task
        //    ShouldThrowServiceExceptionOnExpireRenewImpersonationContextTokensIfServiceErrorOccurredAndLogItAsync()
        //{
        //    // given
        //    Guid someId = Guid.NewGuid();
        //    var serviceException = new Exception();

        //    var failedServiceIdentificationOrchestrationException =
        //        new FailedServiceIdentificationOrchestrationException(
        //            message: "Failed service identification orchestration error occurred, contact support.",
        //            innerException: serviceException);

        //    var expectedIdentificationOrchestrationServiceException =
        //        new IdentificationOrchestrationServiceException(
        //            message: "Identification orchestration service error occurred, contact support.",
        //            innerException: failedServiceIdentificationOrchestrationException);

        //    this.impersonationContextServiceMock.Setup(service =>
        //        service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
        //            .ThrowsAsync(serviceException);

        //    // when
        //    ValueTask<AccessRequest> expireRenewImpersonationContextTokensTask =
        //        this.identificationOrchestrationService
        //            .ExpireRenewImpersonationContextTokensAsync(someId);

        //    IdentificationOrchestrationServiceException
        //        actualIdentificationOrchestrationValidationException =
        //        await Assert.ThrowsAsync<IdentificationOrchestrationServiceException>(
        //            testCode: expireRenewImpersonationContextTokensTask.AsTask);

        //    // then
        //    actualIdentificationOrchestrationValidationException.Should().BeEquivalentTo(
        //        expectedIdentificationOrchestrationServiceException);

        //    this.impersonationContextServiceMock.Verify(service =>
        //        service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
        //            Times.Once);

        //    this.loggingBrokerMock.Verify(broker =>
        //       broker.LogErrorAsync(It.Is(SameExceptionAs(
        //           expectedIdentificationOrchestrationServiceException))),
        //               Times.Once);

        //    this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        //    this.impersonationContextServiceMock.VerifyNoOtherCalls();
        //    this.notificationServiceMock.VerifyNoOtherCalls();
        //    this.accessAuditServiceMock.VerifyNoOtherCalls();
        //    this.documentServiceMock.VerifyNoOtherCalls();
        //    this.loggingBrokerMock.VerifyNoOtherCalls();
        //    this.hashBrokerMock.VerifyNoOtherCalls();
        //    this.dateTimeBrokerMock.VerifyNoOtherCalls();
        //    this.identifierBrokerMock.VerifyNoOtherCalls();
        //}
    }
}
