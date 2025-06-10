// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedAuditStorageException =
                new FailedStorageAuditException(
                    message: "Failed audit storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedAuditDependencyException =
                new AuditDependencyException(
                    message: "Audit dependency error occurred, contact support.",
                        innerException: failedAuditStorageException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAuditsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Audit>> modifyAuditTask =
                this.auditService.RetrieveAllAuditsAsync();

            AuditDependencyException actualAuditDependencyException =
                await Assert.ThrowsAsync<AuditDependencyException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditDependencyException.Should().BeEquivalentTo(
                expectedAuditDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAuditsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceError = new Exception();

            var failedServiceAuditException = new FailedServiceAuditException(
                message: "Failed service audit error occurred, contact support.",
                innerException: serviceError);

            var expectedAuditServiceException = new AuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAuditsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Audit>> retrieveAllAuditsTask =
                this.auditService.RetrieveAllAuditsAsync();

            AuditServiceException actualAuditServiceExcpetion =
                await Assert.ThrowsAsync<AuditServiceException>(
                    testCode: retrieveAllAuditsTask.AsTask);

            // then
            actualAuditServiceExcpetion.Should().BeEquivalentTo(expectedAuditServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAuditsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditServiceException))),
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
