﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Reidentification.Core.Models.Foundations.AccessAudits.Exceptions;
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

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    removeByIdAccessAuditTask.AsTask);

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

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
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

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<AccessAudit> removeAccessAuditByIdTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditDependencyValidationException actualAccessAuditDependencyValidationException =
                await Assert.ThrowsAsync<AccessAuditDependencyValidationException>(
                    removeAccessAuditByIdTask.AsTask);

            // then
            actualAccessAuditDependencyValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
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

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId))
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(randomAccessAuditId);

            AccessAuditServiceException actualAccessAuditServiceExcpetion =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    removeByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceExcpetion.Should().BeEquivalentTo(expectedAccessAuditServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(randomAccessAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
