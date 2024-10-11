// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
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
                broker.SelectAllLookupsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Lookup>> retrieveAllLookupsTask =
                this.lookupService.RetrieveAllLookupsAsync();

            LookupDependencyException actualLookupDependencyException =
                await Assert.ThrowsAsync<LookupDependencyException>(
                    testCode: retrieveAllLookupsTask.AsTask);

            // then
            actualLookupDependencyException.Should()
                .BeEquivalentTo(expectedLookupDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllLookupsAsync(),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedLookupServiceException =
                new FailedLookupServiceException(
                    message: "Failed lookup service occurred, please contact support",
                    innerException: serviceException);

            var expectedLookupServiceException =
                new LookupServiceException(
                    message: "Lookup service error occurred, contact support.",
                    innerException: failedLookupServiceException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllLookupsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<Lookup>> retrieveAllLookupsTask =
                this.lookupService.RetrieveAllLookupsAsync();

            LookupServiceException actualLookupServiceException =
                await Assert.ThrowsAsync<LookupServiceException>(
                    testCode: retrieveAllLookupsTask.AsTask);

            // then
            actualLookupServiceException.Should()
                .BeEquivalentTo(expectedLookupServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllLookupsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}