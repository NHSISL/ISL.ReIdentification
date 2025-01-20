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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdWhenAuditIdIsInvalidAndLogItAsync()
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
            ValueTask<Audit> retrieveByIdAuditTask =
                this.accessAuditService.RetrieveAuditByIdAsync(invalidAuditId);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: retrieveByIdAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAuditId),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfAuditNotFoundAndLogItAsync()
        {
            // given
            Guid someAuditId = Guid.NewGuid();
            Audit nullAudit = null;

            var notFoundAuditException = new NotFoundAuditException(
                message: $"Audit not found with Id: {someAuditId}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(someAuditId))
                    .ReturnsAsync(nullAudit);

            // when
            ValueTask<Audit> retrieveByIdAuditTask =
                this.accessAuditService.RetrieveAuditByIdAsync(someAuditId);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: retrieveByIdAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(someAuditId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))), Times.Once());

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
