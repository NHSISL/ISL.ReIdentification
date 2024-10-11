// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedAccessAuditStorageException =
                new FailedStorageAccessAuditException(
                    message: "Failed access audit storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedAccessAuditDependencyException =
                new AccessAuditDependencyException(
                    message: "Access audit dependency error occurred, contact support.",
                        innerException: failedAccessAuditStorageException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAccessAuditsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<AccessAudit>> modifyAccessAuditTask =
                this.accessAuditService.RetrieveAllAccessAuditsAsync();

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAccessAuditsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyException))),
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

            var failedServiceAccessAuditException = new FailedServiceAccessAuditException(
                message: "Failed service access audit error occurred, contact support.",
                innerException: serviceError);

            var expectedAccessAuditServiceException = new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceAccessAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllAccessAuditsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<AccessAudit>> retrieveAllAccessAuditsTask =
                this.accessAuditService.RetrieveAllAccessAuditsAsync();

            AccessAuditServiceException actualAccessAuditServiceExcpetion =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: retrieveAllAccessAuditsTask.AsTask);

            // then
            actualAccessAuditServiceExcpetion.Should().BeEquivalentTo(expectedAccessAuditServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllAccessAuditsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
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
