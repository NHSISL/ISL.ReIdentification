// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageOdsDataException = new FailedStorageOdsDataException(
                message: "Failed ODS data storage error occurred, contact support.",
                innerException: sqlException);

            var expectedOdsDataDependencyException = new OdsDataDependencyException(
                message: "OdsData dependency error occurred, contact support.",
                innerException: failedStorageOdsDataException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<OdsData>> retrieveAllOdsDatasTask =
                this.odsDataService.RetrieveAllOdsDatasAsync();

            OdsDataDependencyException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataDependencyException>(
                    testCode: retrieveAllOdsDatasTask.AsTask);

            // then
            actualOdsDataDependencyException.Should().BeEquivalentTo(expectedOdsDataDependencyException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedOdsDataDependencyException))),
                        Times.Once());

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceException = new Exception();

            var failedServiceOdsDataException = new FailedServiceOdsDataException(
                message: "Failed service ODS data error occurred, contact support.",
                innerException: serviceException);

            var expectedOdsDataServiceException = new OdsDataServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceOdsDataException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<OdsData>> retrieveAllOdsDatasTask =
                this.odsDataService.RetrieveAllOdsDatasAsync();

            OdsDataServiceException actualOdsDataDependencyException =
                await Assert.ThrowsAsync<OdsDataServiceException>(
                    testCode: retrieveAllOdsDatasTask.AsTask);

            // then
            actualOdsDataDependencyException.Should().BeEquivalentTo(expectedOdsDataServiceException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
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
