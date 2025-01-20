// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Audits
{
    public partial class AuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAuditIsNullAndLogItAsync()
        {
            // given
            Audit nullAudit = null;
            var nullAuditException = new NullAuditException(message: "Audit is null.");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: nullAuditException);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(nullAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAuditValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfAuditIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidAudit = new Audit
            {
                AuditType = invalidText,
                AuditDetail = invalidText,
                LogLevel = invalidText,
            };

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.Id),
                values: "Id is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.AuditType),
                values: "Text is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.AuditDetail),
                values: "Text is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.LogLevel),
                values: "Text is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedDate),
                values: "Date is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedBy),
                values: "Text is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(Audit.CreatedDate)}"
                    });

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: "Text is invalid");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Audit invalidAudit = CreateRandomModifyAudit(randomDateTimeOffset);
            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidAudit.AuditType = GetRandomStringWithLengthOf(256);
            invalidAudit.LogLevel = GetRandomStringWithLengthOf(256);
            invalidAudit.CreatedBy = inputCreatedByUpdatedByString;
            invalidAudit.UpdatedBy = inputCreatedByUpdatedByString;

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.AuditType),
                values: $"Text exceed max length of {invalidAudit.AuditType.Length - 1} characters");

            invalidAuditException.AddData(
                key: nameof(Audit.LogLevel),
                values: $"Text exceed max length of {invalidAudit.LogLevel.Length - 1} characters");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedBy),
                values: $"Text exceed max length of {invalidAudit.CreatedBy.Length - 1} characters");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: $"Text exceed max length of {invalidAudit.UpdatedBy.Length - 1} characters");

            var expectedAuditException = new
                AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModiyIfAuditHasSameCreatedDateUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDatTimeOffset = GetRandomDateTimeOffset();
            Audit randomAudit = CreateRandomAudit(randomDatTimeOffset);
            var invalidAudit = randomAudit;

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: $"Date is the same as {nameof(Audit.CreatedDate)}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditException);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset);
            Audit invalidAudit = randomAudit;
            invalidAudit.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomAudit.UpdatedDate}"
                ]);

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageAuditDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset);
            Audit nonExistingAudit = randomAudit;
            nonExistingAudit.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);
            Audit nullAudit = null;

            var notFoundAuditException = new NotFoundAuditException(
                message: $"Audit not found with Id: {nonExistingAudit.Id}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(nonExistingAudit.Id))
                    .ReturnsAsync(nullAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(nonExistingAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

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

        [Fact]
        public async Task
            ShouldThrowValidationExceptionIfStorageAuditCreatedDateIsNotSameAsAuditCreatedDateAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Audit randomAudit = CreateRandomModifyAudit(randomDateTimeOffset);
            Audit invalidAudit = randomAudit;
            Audit storageAudit = invalidAudit.DeepClone();
            storageAudit.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);
            storageAudit.UpdatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedDate),
                values: $"Date is not the same as {nameof(Audit.CreatedDate)}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id))
                    .ReturnsAsync(storageAudit);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAuditValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Audit randomAudit = CreateRandomModifyAudit(randomDateTimeOffset);
            Audit invalidAudit = randomAudit;
            Audit storageAudit = invalidAudit.DeepClone();
            invalidAudit.UpdatedDate = storageAudit.UpdatedDate;

            var invalidAuditValidationException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditValidationException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: $"Date is the same as {nameof(Audit.UpdatedDate)}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditValidationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id))
                    .ReturnsAsync(storageAudit);

            // when
            ValueTask<Audit> modifyAuditTask =
                this.accessAuditService.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAuditValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
