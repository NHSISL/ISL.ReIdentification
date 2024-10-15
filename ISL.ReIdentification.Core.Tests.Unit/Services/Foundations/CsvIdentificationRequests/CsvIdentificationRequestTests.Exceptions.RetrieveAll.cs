// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageCsvIdentificationRequestException =
                new FailedStorageCsvIdentificationRequestException(
                    message: "Failed CSV identification request storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedCsvIdentificationRequestDependencyException =
                new CsvIdentificationRequestDependencyException(
                    message: "CSV identification request dependency error occurred, contact support.",
                    innerException: failedStorageCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<CsvIdentificationRequest>> retrieveAllCsvIdentificationRequestsTask =
                this.csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync();

            CsvIdentificationRequestDependencyException actualCsvIdentificationRequestDependencyException =
                await Assert.ThrowsAsync<CsvIdentificationRequestDependencyException>(
                    testCode: retrieveAllCsvIdentificationRequestsTask.AsTask);

            // then
            actualCsvIdentificationRequestDependencyException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestDependencyException))),
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

            var failedServiceCsvIdentificationRequestException =
                new FailedServiceCsvIdentificationRequestException(
                    message: "Failed service CSV identification request error occurred, contact support.",
                    innerException: serviceError);

            var expectedCsvIdentificationRequestServiceException =
                new CsvIdentificationRequestServiceException(
                    message: "CSV identification request service error occurred, contact support.",
                    innerException: failedServiceCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<CsvIdentificationRequest>> retrieveAllCsvIdentificationRequestsTask =
                this.csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync();

            CsvIdentificationRequestServiceException actualCsvIdentificationRequestServiceException =
                await Assert.ThrowsAsync<CsvIdentificationRequestServiceException>(
                    testCode: retrieveAllCsvIdentificationRequestsTask.AsTask);

            // then
            actualCsvIdentificationRequestServiceException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestServiceException))),
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