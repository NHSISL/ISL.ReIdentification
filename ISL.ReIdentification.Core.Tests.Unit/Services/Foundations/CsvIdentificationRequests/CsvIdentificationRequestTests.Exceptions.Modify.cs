// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            CsvIdentificationRequest someCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            SqlException sqlException = CreateSqlException();

            var failedCsvIdentificationRequestStorageException =
                new FailedStorageCsvIdentificationRequestException(
                    message: "Failed CSV identification request storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedCsvIdentificationRequestDependencyException =
                new CsvIdentificationRequestDependencyException(
                    message: "CSV identification request dependency error occurred, contact support.",
                        innerException: failedCsvIdentificationRequestStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(someCsvIdentificationRequest);

            CsvIdentificationRequestDependencyException actualCsvIdentificationRequestDependencyException =
                await Assert.ThrowsAsync<CsvIdentificationRequestDependencyException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestDependencyException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequest.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateCsvIdentificationRequestAsync(someCsvIdentificationRequest),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset);

            randomCsvIdentificationRequest.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationCsvIdentificationRequestException =
                new FailedOperationCsvIdentificationRequestException(
                    message: "Failed operation CSV identification request error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedCsvIdentificationRequestDependencyException =
                new CsvIdentificationRequestDependencyException(
                    message: "CSV identification request dependency error occurred, contact support.",
                    innerException: failedOperationCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(randomCsvIdentificationRequest);

            CsvIdentificationRequestDependencyException actualCsvIdentificationRequestDependencyException =
                await Assert.ThrowsAsync<CsvIdentificationRequestDependencyException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestDependencyException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        private async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset, randomEntraUser.EntraUserId);

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedCsvIdentificationRequestException =
                new LockedCsvIdentificationRequestException(
                    message: "Locked CSV identification request record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedCsvIdentificationRequestDependencyValidationException =
                new CsvIdentificationRequestDependencyValidationException(
                    message: "CSV identification request dependency validation error occurred, " +
                        "fix errors and try again.",
                    innerException: lockedCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(
                    randomCsvIdentificationRequest);

            CsvIdentificationRequestDependencyValidationException 
                actualCsvIdentificationRequestDependencyValidationException =
                    await Assert.ThrowsAsync<CsvIdentificationRequestDependencyValidationException>(
                        testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestDependencyValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset, randomEntraUser.EntraUserId);

            randomCsvIdentificationRequest.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var serviceException = new Exception();

            var failedServiceCsvIdentificationRequestException =
                new FailedServiceCsvIdentificationRequestException(
                    message: "Failed service CSV identification request error occurred, contact support.",
                    innerException: serviceException);

            var expectedCsvIdentificationRequestServiceException =
                new CsvIdentificationRequestServiceException(
                    message: "CSV identification request service error occurred, contact support.",
                    innerException: failedServiceCsvIdentificationRequestException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(randomCsvIdentificationRequest);

            CsvIdentificationRequestServiceException actualCsvIdentificationRequestServiceException =
                await Assert.ThrowsAsync<CsvIdentificationRequestServiceException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestServiceException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}