// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
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

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    testCode: removeByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationOnRemoveAccessAuditByIdIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAccessAuditId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedAccessAuditException =
                new LockedAccessAuditException(
                    message: "Locked access audit record error occurred, please try again.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedAccessAuditDependencyValidationException =
                new AccessAuditDependencyValidationException(
                    message: "Access audit dependency validation error occurred, fix errors and try again.",
                    innerException: lockedAccessAuditException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<AccessAudit> removeAccessAuditByIdTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditDependencyValidationException actualAccessAuditDependencyValidationException =
                await Assert.ThrowsAsync<AccessAuditDependencyValidationException>(
                    testCode: removeAccessAuditByIdTask.AsTask);

            // then
            actualAccessAuditDependencyValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditDependencyValidationException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveAccessAuditByIdWhenServiceErrorOccursAndLogItAsync()
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

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditServiceException actualAccessAuditServiceExcpetion =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: removeByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceExcpetion.Should().BeEquivalentTo(expectedAccessAuditServiceException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
