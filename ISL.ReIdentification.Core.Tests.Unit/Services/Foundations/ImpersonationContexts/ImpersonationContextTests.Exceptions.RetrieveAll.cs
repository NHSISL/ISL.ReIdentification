// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageImpersonationContextException =
                new FailedStorageImpersonationContextException(
                    message: "Failed impersonation context storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedImpersonationContextDependencyException =
                new ImpersonationContextDependencyException(
                    message: "Impersonation context dependency error occurred, contact support.",
                    innerException: failedStorageImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllImpersonationContextsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ImpersonationContext>> retrieveAllImpersonationContextsTask =
                this.impersonationContextService.RetrieveAllImpersonationContextsAsync();

            ImpersonationContextDependencyException actualImpersonationContextDependencyException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                    testCode: retrieveAllImpersonationContextsTask.AsTask);

            // then
            actualImpersonationContextDependencyException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllImpersonationContextsAsync(),
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
        public async Task ShouldThrowServiceErrorOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceError = new Exception();

            var failedServiceImpersonationContextException =
                new FailedServiceImpersonationContextException(
                    message: "Failed service impersonation context error occurred, contact support.",
                    innerException: serviceError);

            var expectedImpersonationContextServiceException =
                new ImpersonationContextServiceException(
                    message: "Impersonation context service error occurred, contact support.",
                    innerException: failedServiceImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllImpersonationContextsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<ImpersonationContext>> retrieveAllImpersonationContextsTask =
                this.impersonationContextService.RetrieveAllImpersonationContextsAsync();

            ImpersonationContextServiceException actualImpersonationContextServiceException =
                await Assert.ThrowsAsync<ImpersonationContextServiceException>(
                    testCode: retrieveAllImpersonationContextsTask.AsTask);

            // then
            actualImpersonationContextServiceException.Should().BeEquivalentTo(
                expectedImpersonationContextServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllImpersonationContextsAsync(),
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