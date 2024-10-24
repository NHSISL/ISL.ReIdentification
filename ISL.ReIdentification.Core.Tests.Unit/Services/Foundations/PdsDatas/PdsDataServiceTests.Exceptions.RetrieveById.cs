// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStoragePdsDataException =
                new FailedStoragePdsDataException(
                    message: "Failed pdsData storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedPdsDataDependencyException =
                new PdsDataDependencyException(
                    message: "PdsData dependency error occurred, contact support.",
                    innerException: failedStoragePdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(someId);

            PdsDataDependencyException actualPdsDataDependencyException =
                await Assert.ThrowsAsync<PdsDataDependencyException>(
                    testCode: retrievePdsDataByIdTask.AsTask);

            // then
            actualPdsDataDependencyException.Should()
                .BeEquivalentTo(expectedPdsDataDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedPdsDataDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPdsDataServiceException =
                new FailedPdsDataServiceException(
                    message: "Failed pdsData service occurred, please contact support",
                    innerException: serviceException);

            var expectedPdsDataServiceException =
                new PdsDataServiceException(
                    message: "PdsData service error occurred, contact support.",
                    innerException: failedPdsDataServiceException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(someId);

            PdsDataServiceException actualPdsDataServiceException =
                await Assert.ThrowsAsync<PdsDataServiceException>(
                    testCode: retrievePdsDataByIdTask.AsTask);

            // then
            actualPdsDataServiceException.Should()
                .BeEquivalentTo(expectedPdsDataServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPdsDataServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}