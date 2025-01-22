// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdAuditWithInvalidIdAndLogItAsync()
        {
            // given
            Guid invalidAuditId = Guid.Empty;

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.Id),
                values: "Id is invalid");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            // when
            ValueTask<Audit> removeByIdAuditTask = this.accessAuditService
                .RemoveAuditByIdAsync(invalidAuditId);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: removeByIdAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAuditValidationException))),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAuditId),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfStorageAuditDoesNotExistAndLogItAsync()
        {
            // given
            Audit randomAudit = CreateRandomAudit();
            Audit nonExistingAudit = randomAudit;
            Audit nullAudit = null;

            var notFoundAuditException = new NotFoundAuditException(
                message: $"Audit not found with Id: {nonExistingAudit.Id}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(nonExistingAudit.Id))
                    .ReturnsAsync(nullAudit);

            // when
            ValueTask<Audit> removeByIdAuditTask =
                this.accessAuditService.RemoveAuditByIdAsync(nonExistingAudit.Id);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: removeByIdAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(nonExistingAudit.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
