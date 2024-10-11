// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveOdsDataByIdByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid randomOdsDataId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStorageOdsDataException = new FailedStorageOdsDataException(
                message: "Failed ODS data storage error occurred, contact support.",
                innerException: sqlException);

            var expectedOdsDataDependencyException = new OdsDataDependencyException(
                message: "OdsData dependency error occurred, contact support.",
                innerException: failedStorageOdsDataException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsDataId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<OdsData> retrieveByIdOdsDataTask =
                this.odsDataService.RetrieveOdsDataByIdAsync(randomOdsDataId);

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: retrieveByIdOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should().BeEquivalentTo(expectedOdsDataDependencyException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsDataId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once());

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveOdsDataByIdWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid randomOdsDataId = Guid.NewGuid();
            Exception serviceException = new Exception();

            var failedServiceOdsDataException = new FailedServiceOdsDataException(
                message: "Failed service ODS data error occurred, contact support.",
                innerException: serviceException);

            var expectedOdsDataServiceException = new OdsDataServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceOdsDataException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsDataId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<OdsData> retrieveByIdOdsDataTask =
                this.odsDataService.RetrieveOdsDataByIdAsync(randomOdsDataId);

            OdsDataServiceException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataServiceException>(
                    testCode: retrieveByIdOdsDataTask.AsTask);

            // then
            actualOdsDataDependencyException.Should().BeEquivalentTo(expectedOdsDataServiceException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectOdsDataByIdAsync(randomOdsDataId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataServiceException))),
                        Times.Once());

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
