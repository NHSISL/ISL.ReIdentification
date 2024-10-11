// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            SqlException sqlException = CreateSqlException();

            var failedStorageLookupException =
                new FailedStorageLookupException(
                    message: "Failed lookup storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedLookupDependencyException =
                new LookupDependencyException(
                    message: "Lookup dependency error occurred, contact support.",
                    innerException: failedStorageLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(randomLookup.Id))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.RemoveLookupByIdAsync(randomLookup.Id);

            LookupDependencyException actualLookupDependencyException =
                await Assert.ThrowsAsync<LookupDependencyException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupDependencyException.Should()
                .BeEquivalentTo(expectedLookupDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(randomLookup.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedLookupDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someLookupId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedLookupException =
                new LockedLookupException(
                    message: "Locked lookup record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedLookupDependencyValidationException =
                new LookupDependencyValidationException(
                    message: "Lookup dependency validation occurred, please try again.",
                    innerException: lockedLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Lookup> removeLookupByIdTask =
                this.lookupService.RemoveLookupByIdAsync(someLookupId);

            LookupDependencyValidationException actualLookupDependencyValidationException =
                await Assert.ThrowsAsync<LookupDependencyValidationException>(
                    testCode: removeLookupByIdTask.AsTask);

            // then
            actualLookupDependencyValidationException.Should()
                .BeEquivalentTo(expectedLookupDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someLookupId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStorageLookupException =
                new FailedStorageLookupException(
                    message: "Failed lookup storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedLookupDependencyException =
                new LookupDependencyException(
                    message: "Lookup dependency error occurred, contact support.",
                    innerException: failedStorageLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Lookup> deleteLookupTask =
                this.lookupService.RemoveLookupByIdAsync(someLookupId);

            LookupDependencyException actualLookupDependencyException =
                await Assert.ThrowsAsync<LookupDependencyException>(
                    testCode: deleteLookupTask.AsTask);

            // then
            actualLookupDependencyException.Should()
                .BeEquivalentTo(expectedLookupDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedLookupDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someLookupId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedLookupServiceException =
                new FailedLookupServiceException(
                    message: "Failed lookup service occurred, please contact support",
                    innerException: serviceException);

            var expectedLookupServiceException =
                new LookupServiceException(
                    message: "Lookup service error occurred, contact support.",
                    innerException: failedLookupServiceException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Lookup> removeLookupByIdTask =
                this.lookupService.RemoveLookupByIdAsync(someLookupId);

            LookupServiceException actualLookupServiceException =
                await Assert.ThrowsAsync<LookupServiceException>(
                    testCode: removeLookupByIdTask.AsTask);

            // then
            actualLookupServiceException.Should()
                .BeEquivalentTo(expectedLookupServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}