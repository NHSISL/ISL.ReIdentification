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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();
            SqlException sqlException = CreateSqlException();

            var failedStorageOdsDataException =
                new FailedStorageOdsDataException(
                    message: "Failed odsData storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedOdsDataDependencyException =
                new OdsDataDependencyException(
                    message: "OdsData dependency error occurred, contact support.",
                    innerException: failedStorageOdsDataException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<OdsData> addOdsDataTask =
                this.odsDataService.AddOdsDataAsync(someOdsData);

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: addOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);
            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Never);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfOdsDataAlreadyExistsAndLogItAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData alreadyExistsOdsData = randomOdsData;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsOdsDataException =
                new AlreadyExistsOdsDataException(
                    message: "OdsData with the same Id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedOdsDataDependencyValidationException =
                new OdsDataDependencyValidationException(
                    message: "OdsData dependency validation occurred, please try again.",
                    innerException: alreadyExistsOdsDataException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<OdsData> addOdsDataTask =
                this.odsDataService.AddOdsDataAsync(alreadyExistsOdsData);

            // then
            OdsDataDependencyValidationException actualOdsDataDependencyValidationException =
                await Assert.ThrowsAsync<OdsDataDependencyValidationException>(
                    testCode: addOdsDataTask.AsTask);

            actualOdsDataDependencyValidationException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
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

            var expectedOdsDataValidationException =
                new OdsDataDependencyValidationException(
                    message: "OdsData dependency validation occurred, please try again.",
                    innerException: invalidOdsDataReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<OdsData> addOdsDataTask =
                this.odsDataService.AddOdsDataAsync(someOdsData);

            // then
            OdsDataDependencyValidationException actualOdsDataDependencyValidationException =
                await Assert.ThrowsAsync<OdsDataDependencyValidationException>(
                    testCode: addOdsDataTask.AsTask);

            actualOdsDataDependencyValidationException.Should()
                .BeEquivalentTo(expectedOdsDataValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataValidationException))),
                        Times.Once);
            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(someOdsData),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();

            var databaseUpdateException =
                new DbUpdateException();

            var failedOperationOdsDataException =
                new FailedOperationOdsDataException(
                    message: "Failed odsData operation error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedOdsDataDependencyException =
                new OdsDataDependencyException(
                    message: "OdsData dependency error occurred, contact support.",
                    innerException: failedOperationOdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(databaseUpdateException);

            // when
            ValueTask<OdsData> addOdsDataTask =
                this.odsDataService.AddOdsDataAsync(someOdsData);

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: addOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should()
                .BeEquivalentTo(expectedOdsDataDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            OdsData someOdsData = CreateRandomOdsData();
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
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<OdsData> addOdsDataTask =
                this.odsDataService.AddOdsDataAsync(someOdsData);

            OdsDataServiceException actualOdsDataServiceException =
                await Assert.ThrowsAsync<OdsDataServiceException>(
                    testCode: addOdsDataTask.AsTask);

            // then
            actualOdsDataServiceException.Should()
                .BeEquivalentTo(expectedOdsDataServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}