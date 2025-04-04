// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            SqlException sqlException = CreateSqlException();

            var failedStorageOdsDataException =
                new FailedStorageOdsDataException(
                    message: "Failed odsData storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedOdsDataDependencyException =
                new OdsDataDependencyException(
                    message: "OdsData dependency error occurred, contact support.",
                    innerException: failedStorageOdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(randomOdsData);

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(randomOdsData),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidOdsDataReferenceException =
                new InvalidOdsDataReferenceException(
                    message: "Invalid odsData reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            OdsDataDependencyValidationException expectedOdsDataDependencyValidationException =
                new OdsDataDependencyValidationException(
                    message: "OdsData dependency validation occurred, please try again.",
                    innerException: invalidOdsDataReferenceException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(someOdsData.Id))
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(someOdsData);

            OdsDataDependencyValidationException actualOdsDataDependencyValidationException =
                await Assert.ThrowsAsync<OdsDataDependencyValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyValidationException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(someOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedOdsDataDependencyValidationException))),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(someOdsData),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            var databaseUpdateException = new DbUpdateException();

            var failedOperationOdsDataException =
                new FailedOperationOdsDataException(
                    message: "Failed odsData operation error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedOdsDataDependencyException =
                new OdsDataDependencyException(
                    message: "OdsData dependency error occurred, contact support.",
                    innerException: failedOperationOdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(randomOdsData);

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(randomOdsData),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOdsDataException =
                new LockedOdsDataException(
                    message: "Locked odsData record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedOdsDataDependencyValidationException =
                new OdsDataDependencyValidationException(
                    message: "OdsData dependency validation occurred, please try again.",
                    innerException: lockedOdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(randomOdsData);

            OdsDataDependencyValidationException actualOdsDataDependencyValidationException =
                await Assert.ThrowsAsync<OdsDataDependencyValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyValidationException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(randomOdsData),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            var serviceException = new Exception();

            var failedOdsDataServiceException =
                new FailedOdsDataServiceException(
                    message: "Failed odsData service occurred, please contact support",
                    innerException: serviceException);

            var expectedOdsDataServiceException =
                new OdsDataServiceException(
                    message: "OdsData service error occurred, contact support.",
                    innerException: failedOdsDataServiceException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(randomOdsData);

            OdsDataServiceException actualOdsDataServiceException =
                await Assert.ThrowsAsync<OdsDataServiceException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataServiceException.Should()
                .BeEquivalentTo(expectedOdsDataServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(randomOdsData),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}