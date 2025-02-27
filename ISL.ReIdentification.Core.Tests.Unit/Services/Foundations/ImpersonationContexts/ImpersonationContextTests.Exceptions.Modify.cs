// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ImpersonationContext someImpersonationContext = CreateRandomImpersonationContext();
            SqlException sqlException = CreateSqlException();

            var failedImpersonationContextStorageException =
                new FailedStorageImpersonationContextException(
                    message: "Failed impersonation context storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedImpersonationContextDependencyException =
                new ImpersonationContextDependencyException(
                    message: "Impersonation context dependency error occurred, contact support.",
                        innerException: failedImpersonationContextStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(someImpersonationContext);

            ImpersonationContextDependencyException actualImpersonationContextDependencyException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(someImpersonationContext.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(someImpersonationContext),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            var databaseUpdateException = new DbUpdateException();

            var dbUpdateException = new DbUpdateException();

            var failedOperationImpersonationContextException =
                new FailedOperationImpersonationContextException(
                    message: "Failed operation impersonation context error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedImpersonationContextDependencyException =
                new ImpersonationContextDependencyException(
                    message: "Impersonation context dependency error occurred, contact support.",
                    innerException: failedOperationImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(randomImpersonationContext);

            ImpersonationContextDependencyException actualImpersonationContextDependencyException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(randomImpersonationContext),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(randomImpersonationContext.Id),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedImpersonationContextException =
                new LockedImpersonationContextException(
                    message: "Locked impersonation context record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedImpersonationContextDependencyValidationException =
                new ImpersonationContextDependencyValidationException(
                    message: "Impersonation context dependency validation error occurred, fix errors and try again.",
                    innerException: lockedImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(randomImpersonationContext);

            ImpersonationContextDependencyValidationException actualImpersonationContextDependencyValidationException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyValidationException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(randomImpersonationContext),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(randomImpersonationContext.Id),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            var serviceException = new Exception();

            var failedServiceImpersonationContextException =
                new FailedServiceImpersonationContextException(
                    message: "Failed service impersonation context error occurred, contact support.",
                    innerException: serviceException);

            var expectedImpersonationContextServiceException =
                new ImpersonationContextServiceException(
                    message: "Impersonation context service error occurred, contact support.",
                    innerException: failedServiceImpersonationContextException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectImpersonationContextByIdAsync(randomImpersonationContext.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ImpersonationContext> modifyImpersonationContextTask =
                this.impersonationContextService.ModifyImpersonationContextAsync(randomImpersonationContext);

            ImpersonationContextServiceException actualImpersonationContextServiceException =
                await Assert.ThrowsAsync<ImpersonationContextServiceException>(
                    testCode: modifyImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextServiceException.Should().BeEquivalentTo(
                expectedImpersonationContextServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectImpersonationContextByIdAsync(randomImpersonationContext.Id),
                    Times.Never());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateImpersonationContextAsync(randomImpersonationContext),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}