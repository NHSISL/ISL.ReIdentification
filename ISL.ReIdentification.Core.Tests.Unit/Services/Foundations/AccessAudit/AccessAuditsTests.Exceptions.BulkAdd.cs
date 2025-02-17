// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkAddIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            List<AccessAudit> someAccessAudits = new List<AccessAudit> { CreateRandomAccessAudit() };
            SqlException sqlException = CreateSqlException();

            var failedStorageAccessAuditException =
                new FailedStorageAccessAuditException(
                    message: "Failed access audit storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedAccessAuditDependencyException =
                new AccessAuditDependencyException(
                    message: "Access audit dependency error occurred, contact support.",
                    innerException: failedStorageAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask addAccessAuditTask =
                this.accessAuditService.BulkAddAccessAuditAsync(
                    someAccessAudits);

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddIfAccessAuditAlreadyExistsAndLogItAsync()
        {
            // given
            List<AccessAudit> someAccessAudits = new List<AccessAudit> { CreateRandomAccessAudit() };

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsAccessAuditException =
                new AlreadyExistsAccessAuditException(
                    message: "Access audit already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedAccessAuditDependencyValidationException =
                new AccessAuditDependencyValidationException(
                    message: "Access audit dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask addAccessAuditTask =
                this.accessAuditService.BulkAddAccessAuditAsync(someAccessAudits);

            AccessAuditDependencyValidationException actualAccessAuditDependencyValidationException =
                await Assert.ThrowsAsync<AccessAuditDependencyValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyValidationException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBulkAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            List<AccessAudit> someAccessAudits = new List<AccessAudit> { CreateRandomAccessAudit() };
            var dbUpdateException = new DbUpdateException();

            var failedOperationAccessAuditException =
                new FailedOperationAccessAuditException(
                    message: "Failed operation access audit error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedAccessAuditDependencyException =
                new AccessAuditDependencyException(
                    message: "Access audit dependency error occurred, contact support.",
                    innerException: failedOperationAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask addAccessAuditTask =
                this.accessAuditService.BulkAddAccessAuditAsync(
                    someAccessAudits);

            AccessAuditDependencyException actualAccessAuditDependencyException =
                await Assert.ThrowsAsync<AccessAuditDependencyException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditDependencyException.Should().BeEquivalentTo(
                expectedAccessAuditDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            List<AccessAudit> someAccessAudits = new List<AccessAudit> { CreateRandomAccessAudit() };
            var serviceException = new Exception();

            var failedServiceAccessAuditException =
                new FailedServiceAccessAuditException(
                    message: "Failed service access audit error occurred, contact support.",
                    innerException: serviceException);

            var expectedAccessAuditServiceException =
                new AccessAuditServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask addAccessAuditTask =
                this.accessAuditService.BulkAddAccessAuditAsync(someAccessAudits);

            AccessAuditServiceException actualAccessAuditServiceException =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceException.Should().BeEquivalentTo(
                expectedAccessAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
