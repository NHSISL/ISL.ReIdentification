// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            var someImpersonationContextId = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageImpersonationContextException =
                new FailedStorageImpersonationContextException(
                    message: "Failed impersonation context storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedImpersonationContextDependencyException =
                new ImpersonationContextDependencyException(
                    message: "ImpersonationContext dependency error occurred, contact support.",
                    innerException: failedStorageImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(someImpersonationContextId))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<ImpersonationContext> retrieveImpersonationContextByIdTask =
                this.impersonationContextService.RetrieveImpersonationContextByIdAsync(someImpersonationContextId);

            // then
            await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                testCode: retrieveImpersonationContextByIdTask.AsTask);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(someImpersonationContextId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            //given
            var someImpersonationContextId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceImpersonationContextException =
                new FailedServiceImpersonationContextException(
                    message: "Failed service impersonation context error occurred, contact support.",
                    innerException: serviceException);

            var expectedImpersonationContextServiceException =
                new ImpersonationContextServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<ImpersonationContext> retrieveImpersonationContextByIdTask =
                this.impersonationContextService.RetrieveImpersonationContextByIdAsync(someImpersonationContextId);

            ImpersonationContextServiceException actualImpersonationContextServiceException =
                await Assert.ThrowsAsync<ImpersonationContextServiceException>(
                    testCode: retrieveImpersonationContextByIdTask.AsTask);

            //then
            actualImpersonationContextServiceException.Should().BeEquivalentTo(
                expectedImpersonationContextServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogErrorAsync(It.Is(SameExceptionAs(
                expectedImpersonationContextServiceException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}