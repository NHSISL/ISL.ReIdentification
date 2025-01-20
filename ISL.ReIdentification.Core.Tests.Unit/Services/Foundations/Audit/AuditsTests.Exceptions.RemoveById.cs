// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAuditId = Guid.NewGuid();
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
                broker.SelectAuditByIdAsync(randomAuditId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Audit> removeByIdAuditTask =
                this.accessAuditService.RemoveAuditByIdAsync(randomAuditId);

            AuditDependencyException actualAuditDependencyException =
                await Assert.ThrowsAsync<AuditDependencyException>(
                    testCode: removeByIdAuditTask.AsTask);

            // then
            actualAuditDependencyException.Should().BeEquivalentTo(
                expectedAuditDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(randomAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAuditAsync(It.IsAny<Audit>()),
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
            ShouldThrowDependencyValidationOnRemoveAuditByIdIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAuditId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedAuditException =
                new LockedAuditException(
                    message: "Locked audit record error occurred, please try again.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedAuditDependencyValidationException =
                new AuditDependencyValidationException(
                    message: "Audit dependency validation error occurred, fix errors and try again.",
                    innerException: lockedAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Audit> removeAuditByIdTask =
                this.accessAuditService.RemoveAuditByIdAsync(randomAuditId);

            AuditDependencyValidationException actualAuditDependencyValidationException =
                await Assert.ThrowsAsync<AuditDependencyValidationException>(
                    testCode: removeAuditByIdTask.AsTask);

            // then
            actualAuditDependencyValidationException.Should()
                .BeEquivalentTo(expectedAuditDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveAuditByIdWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid randomAuditId = Guid.NewGuid();
            Exception serviceError = new Exception();

            var failedServiceAuditException = new FailedServiceAuditException(
                message: "Failed service audit error occurred, contact support.",
                innerException: serviceError);

            var expectedAuditServiceException = new AuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(randomAuditId))
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<Audit> removeByIdAuditTask =
                this.accessAuditService.RemoveAuditByIdAsync(randomAuditId);

            AuditServiceException actualAuditServiceExcpetion =
                await Assert.ThrowsAsync<AuditServiceException>(
                    testCode: removeByIdAuditTask.AsTask);

            // then
            actualAuditServiceExcpetion.Should().BeEquivalentTo(expectedAuditServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(randomAuditId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteAuditAsync(It.IsAny<Audit>()),
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
