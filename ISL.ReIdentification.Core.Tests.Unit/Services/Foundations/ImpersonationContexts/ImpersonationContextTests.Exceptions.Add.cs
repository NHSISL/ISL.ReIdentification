// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            ImpersonationContext someImpersonationContext = CreateRandomImpersonationContext();
            SqlException sqlException = CreateSqlException();

            var failedStorageImpersonationContextException =
                new FailedStorageImpersonationContextException(
                    message: "Failed impersonation context storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedImpersonationContextDependencyException =
                new ImpersonationContextDependencyException(
                    message: "Impersonation context dependency error occurred, contact support.",
                    innerException: failedStorageImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ImpersonationContext> addImpersonationContextTask =
                this.impersonationContextService.AddImpersonationContextAsync(
                    someImpersonationContext);

            ImpersonationContextDependencyException actualImpersonationContextDependencyException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                    testCode: addImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfImpersonationContextAlreadyExistsAndLogItAsync()
        {
            // given
            ImpersonationContext someImpersonationContext = CreateRandomImpersonationContext();

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsImpersonationContextException =
                new AlreadyExistsImpersonationContextException(
                    message: "Impersonation context already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedImpersonationContextDependencyValidationException =
                new ImpersonationContextDependencyValidationException(
                    message: "Impersonation context dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ImpersonationContext> addImpersonationContextTask =
                this.impersonationContextService.AddImpersonationContextAsync(someImpersonationContext);

            ImpersonationContextDependencyValidationException actualImpersonationContextDependencyValidationException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyValidationException>(
                    testCode: addImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyValidationException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            ImpersonationContext someImpersonationContext = CreateRandomImpersonationContext();
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
            ValueTask<ImpersonationContext> addImpersonationContextTask =
                this.impersonationContextService.AddImpersonationContextAsync(
                    someImpersonationContext);

            ImpersonationContextDependencyException actualImpersonationContextDependencyException =
                await Assert.ThrowsAsync<ImpersonationContextDependencyException>(
                    testCode: addImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextDependencyException.Should().BeEquivalentTo(
                expectedImpersonationContextDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            //given
            ImpersonationContext someImpersonationContext = CreateRandomImpersonationContext();
            var serviceException = new Exception();

            var failedServiceImpersonationContextException =
                new FailedServiceImpersonationContextException(
                    message: "Failed service impersonation context error occurred, contact support.",
                    innerException: serviceException);

            var expectedImpersonationContextServiceException =
                new ImpersonationContextServiceException(
                    message: "Impersonation context service error occurred, contact support.",
                    innerException: failedServiceImpersonationContextException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<ImpersonationContext> addImpersonationContextTask =
                this.impersonationContextService.AddImpersonationContextAsync(someImpersonationContext);

            ImpersonationContextServiceException actualImpersonationContextServiceException =
                await Assert.ThrowsAsync<ImpersonationContextServiceException>(
                    testCode: addImpersonationContextTask.AsTask);

            // then
            actualImpersonationContextServiceException.Should().BeEquivalentTo(
                expectedImpersonationContextServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedImpersonationContextServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertImpersonationContextAsync(It.IsAny<ImpersonationContext>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}