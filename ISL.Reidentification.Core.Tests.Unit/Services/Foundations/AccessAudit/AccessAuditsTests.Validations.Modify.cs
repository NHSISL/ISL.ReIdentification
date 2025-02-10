// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAccessAuditIsNullAndLogItAsync()
        {
            // given
            AccessAudit nullAccessAudit = null;
            var nullAccessAuditException = new NullAccessAuditException(message: "Access audit is null.");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: nullAccessAuditException);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(nullAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfAccessAuditIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidAccessAudit = new AccessAudit
            {
                PseudoIdentifier = invalidText,
                Email = invalidText,
            };

            var invalidAccessAuditException =
                new InvalidAccessAuditException(
                    message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Id),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.RequestId),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.EntraUserId),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.AuditType),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values: "Date is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(AccessAudit.CreatedDate)}"
                    });

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: "Text is invalid");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAccessAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit invalidAccessAudit = CreateRandomModifyAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.PseudoIdentifier = GetRandomStringWithLengthOf(11);
            invalidAccessAudit.Email = GetRandomStringWithLengthOf(321);
            invalidAccessAudit.AuditType = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.CreatedBy = inputCreatedByUpdatedByString;
            invalidAccessAudit.UpdatedBy = inputCreatedByUpdatedByString;

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: $"Text exceed max length of {invalidAccessAudit.PseudoIdentifier.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: $"Text exceed max length of {invalidAccessAudit.Email.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.AuditType),
                values: $"Text exceed max length of {invalidAccessAudit.AuditType.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values: $"Text exceed max length of {invalidAccessAudit.CreatedBy.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text exceed max length of {invalidAccessAudit.UpdatedBy.Length - 1} characters");

            var expectedAccessAuditException = new
                AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModiyIfAccessAuditHasSameCreatedDateUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            var invalidAccessAudit = randomAccessAudit;

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: $"Date is the same as {nameof(AccessAudit.CreatedDate)}");

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditVaildationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditVaildationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAccessAuditValidationException))),
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
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit invalidAccessAudit = randomAccessAudit;
            invalidAccessAudit.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomAccessAudit.UpdatedDate}"
                ]);

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: invalidAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditVaildationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditVaildationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAccessAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageAccessAuditDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit nonExistingAccessAudit = randomAccessAudit;
            nonExistingAccessAudit.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);
            AccessAudit nullAccessAudit = null;

            var notFoundAccessAuditException = new NotFoundAccessAuditException(
                message: $"Access audit not found with Id: {nonExistingAccessAudit.Id}");

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAccessAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(nonExistingAccessAudit.Id))
                    .ReturnsAsync(nullAccessAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(nonExistingAccessAudit);

            AccessAuditValidationException actualAccessAuditVaildationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditVaildationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

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

        [Fact]
        public async Task
            ShouldThrowValidationExceptionIfStorageAccessAuditCreatedDateIsNotSameAsAccessAuditCreatedDateAndLogItAsync()
        {
            // given
            int randomNegativeNumber = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomModifyAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit invalidAccessAudit = randomAccessAudit;
            AccessAudit storageAccessAudit = invalidAccessAudit.DeepClone();
            storageAccessAudit.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);
            storageAccessAudit.UpdatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values: $"Date is not the same as {nameof(AccessAudit.CreatedDate)}");

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: invalidAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(invalidAccessAudit.Id))
                    .ReturnsAsync(storageAccessAudit);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(invalidAccessAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))),
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
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomModifyAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit invalidAccessAudit = randomAccessAudit;
            AccessAudit storageAccessAudit = invalidAccessAudit.DeepClone();
            invalidAccessAudit.UpdatedDate = storageAccessAudit.UpdatedDate;

            var invalidAccessAuditValidationException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditValidationException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: $"Date is the same as {nameof(AccessAudit.UpdatedDate)}");

            var expectedAccessAuditValidationException = new AccessAuditValidationException(
                message: "Access audit validation error occurred, please fix errors and try again.",
                innerException: invalidAccessAuditValidationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAccessAuditByIdAsync(invalidAccessAudit.Id))
                    .ReturnsAsync(storageAccessAudit);

            // when
            ValueTask<AccessAudit> modifyAccessAuditTask =
                this.accessAuditService.ModifyAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: modifyAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAccessAuditByIdAsync(invalidAccessAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
