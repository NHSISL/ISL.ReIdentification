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
        public async Task ShouldThrowValidationExceptionOnAddAuditAsync()
        {
            // given
            Audit nullAudit = null;
            var nullAuditException = new NullAuditException(message: "Audit is null.");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: nullAuditException);

            // when
            ValueTask<Audit> addAuditTask = this.accessAuditService.AddAuditAsync(nullAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(testCode: addAuditTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnAddIfAuditIsInvalidAndLogItAsync(string invalidText)
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
                values: "Date is invalid");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: "Text is invalid");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            // when
            ValueTask<Audit> addAuditTask =
                this.accessAuditService.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(testCode: addAuditTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnAddIfAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Audit invalidAudit = CreateRandomAudit(dateTimeOffset: randomDateTimeOffset);
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

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Audit> addAuditTask =
                this.accessAuditService.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: addAuditTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Audit randomAudit = CreateRandomAudit(now);
            Audit invalidAudit = randomAudit;
            invalidAudit.CreatedBy = GetRandomString();
            invalidAudit.UpdatedBy = GetRandomString();
            invalidAudit.CreatedDate = now;
            invalidAudit.UpdatedDate = GetRandomDateTimeOffset();

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values: $"Text is not the same as {nameof(Audit.CreatedBy)}");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: $"Date is not the same as {nameof(Audit.CreatedDate)}");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Audit> addAuditTask =
                this.accessAuditService.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(
                expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            Audit randomAudit = CreateRandomAudit();
            Audit invalidAudit = randomAudit;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidAudit.CreatedDate = invalidDate;
            invalidAudit.UpdatedDate = invalidDate;

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
            key: nameof(Audit.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Audit> addAuditTask =
                this.accessAuditService.AddAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: addAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(
                expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAuditValidationException))),
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
