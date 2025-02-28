// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAccessAuditId = Guid.NewGuid();
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
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<AccessAudit> retrieveByIdAccessAuditTask =
                this.accessAuditService.RetrieveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    testCode: retrieveByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAccessAuditId = Guid.NewGuid();
            Exception serviceError = new Exception();

            var failedServiceAccessAuditException = new FailedServiceAccessAuditException(
                message: "Failed service access audit error occurred, contact support.",
                innerException: serviceError);

            var expectedAccessAuditServiceException = new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceAccessAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<AccessAudit> retrieveByIdAccessAuditTask =
                this.accessAuditService.RetrieveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditServiceException actualAccessAuditServiceExcpetion =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: retrieveByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceExcpetion.Should().BeEquivalentTo(expectedAccessAuditServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
