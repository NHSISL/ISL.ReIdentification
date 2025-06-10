// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            Audit someAudit = CreateRandomAudit();
            SqlException sqlException = CreateSqlException();

            var failedStorageAuditException =
                new FailedStorageAuditException(
                    message: "Failed audit storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedAuditDependencyException =
                new AuditDependencyException(
                    message: "Audit dependency error occurred, contact support.",
                    innerException: failedStorageAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Audit> addAuditTask =
                this.auditService.AddAuditAsync(
                    someAudit);

            AuditDependencyException actualAuditDependencyException =
                await Assert.ThrowsAsync<AuditDependencyException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditDependencyException.Should().BeEquivalentTo(
                expectedAuditDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfAuditAlreadyExistsAndLogItAsync()
        {
            // given
            Audit someAudit = CreateRandomAudit();

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsAuditException =
                new AlreadyExistsAuditException(
                    message: "Audit already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedAuditDependencyValidationException =
                new AuditDependencyValidationException(
                    message: "Audit dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Audit> addAuditTask =
                this.auditService.AddAuditAsync(someAudit);

            AuditDependencyValidationException actualAuditDependencyValidationException =
                await Assert.ThrowsAsync<AuditDependencyValidationException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditDependencyValidationException.Should().BeEquivalentTo(
                expectedAuditDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            Audit someAudit = CreateRandomAudit();
            var dbUpdateException = new DbUpdateException();

            var failedOperationAuditException =
                new FailedOperationAuditException(
                    message: "Failed operation audit error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedAuditDependencyException =
                new AuditDependencyException(
                    message: "Audit dependency error occurred, contact support.",
                    innerException: failedOperationAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<Audit> addAuditTask =
                this.auditService.AddAuditAsync(
                    someAudit);

            AuditDependencyException actualAuditDependencyException =
                await Assert.ThrowsAsync<AuditDependencyException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditDependencyException.Should().BeEquivalentTo(
                expectedAuditDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            Audit someAudit = CreateRandomAudit();
            var serviceException = new Exception();

            var failedServiceAuditException =
                new FailedServiceAuditException(
                    message: "Failed service audit error occurred, contact support.",
                    innerException: serviceException);

            var expectedAuditServiceException =
                new AuditServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Audit> addAuditTask =
                this.auditService.AddAuditAsync(someAudit);

            AuditServiceException actualAuditServiceException =
                await Assert.ThrowsAsync<AuditServiceException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditServiceException.Should().BeEquivalentTo(
                expectedAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
