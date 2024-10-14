// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdAccessAuditWithInvalidIdAndLogItAsync()
        {
            // given
            Guid invalidAccessAuditId = Guid.Empty;

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Id),
                values: "Id is invalid");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask = this.accessAuditService
                .RemoveAccessAuditByIdAsync(invalidAccessAuditId);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: removeByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(invalidAccessAuditId),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfStorageAccessAuditDoesNotExistAndLogItAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit nonExistingAccessAudit = randomAccessAudit;
            AccessAudit nullAccessAudit = null;

            var notFoundAccessAuditException = new NotFoundAccessAuditException(
                message: $"Access audit not found with Id: {nonExistingAccessAudit.Id}");

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAccessAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(nonExistingAccessAudit.Id))
                    .ReturnsAsync(nullAccessAudit);

            // when
            ValueTask<AccessAudit> removeByIdAccessAuditTask =
                this.accessAuditService.RemoveAccessAuditByIdAsync(nonExistingAccessAudit.Id);

            AccessAuditValidationException actualAccessAuditVaildationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: removeByIdAccessAuditTask.AsTask);

            // then
            actualAccessAuditVaildationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(nonExistingAccessAudit.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAccessAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
