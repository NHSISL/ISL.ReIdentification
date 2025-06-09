// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.Audits;
using Moq;
using Xunit;

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
                this.auditService.ModifyAuditAsync(nullAudit);

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
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

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
                        $"Date is the same as {nameof(Audit.CreatedDate)}",
                        "Date is not recent",
                    });

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidAudit.UpdatedBy}'."
                ]);

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
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
                values:
                [ 
                    $"Text exceed max length of {invalidAudit.UpdatedBy.Length - 1} characters",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidAudit.UpdatedBy}'."
                ]);

            var expectedAuditException = new
                AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAuditException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAuditAsync(It.IsAny<Audit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModiyIfAuditHasSameCreatedDateUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
            var invalidAudit = randomAudit;

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values: $"Date is the same as {nameof(Audit.CreatedDate)}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
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
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
            Audit invalidAudit = randomAudit;
            invalidAudit.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidAuditException = new InvalidAuditException(
                message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.UpdatedDate),
                values:
                [
                    $"Date is not recent"
                ]);

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: invalidAuditException);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
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
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Audit randomAudit = CreateRandomAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
            Audit nonExistingAudit = randomAudit;
            nonExistingAudit.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeNumber);
            Audit nullAudit = null;

            var notFoundAuditException = new NotFoundAuditException(
                message: $"Audit not found with Id: {nonExistingAudit.Id}");

            var expectedAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: notFoundAuditException);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(nonExistingAudit))
                    .ReturnsAsync(nonExistingAudit);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(nonExistingAudit.Id))
                    .ReturnsAsync(nullAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(nonExistingAudit);

            AuditValidationException actualAuditVaildationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditVaildationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(nonExistingAudit.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedAuditValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionIfStorageAuditCreatedDateIsNotSameAsAuditCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            Audit randomAudit =
                CreateRandomModifyAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);

            Audit invalidAudit = randomAudit.DeepClone();
            Audit storageAudit = invalidAudit.DeepClone();
            storageAudit.CreatedDate = storageAudit.CreatedDate.AddMinutes(randomMinutes);
            storageAudit.UpdatedDate = storageAudit.UpdatedDate.AddMinutes(randomMinutes);

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAuditException =
                new InvalidAuditException(
                    message: "Invalid audit. Please correct the errors and try again.");

            invalidAuditException.AddData(
                key: nameof(Audit.CreatedDate),
                values: $"Date is not the same as {nameof(Audit.CreatedDate)}");

            var expectedAuditValidationException =
                new AuditValidationException(
                    message: "Audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAuditException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id))
                    .ReturnsAsync(storageAudit);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should()
                .BeEquivalentTo(expectedAuditValidationException);

            auditServiceMock.Verify(service =>
                service.ApplyModifyAuditAsync(invalidAudit),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAuditValidationException))),
                       Times.Once);

            auditServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Audit randomAudit = CreateRandomModifyAudit(randomDateTimeOffset, randomEntraUser.EntraUserId);
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

            var auditServiceMock = new Mock<AuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            auditServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidAudit))
                    .ReturnsAsync(invalidAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id))
                    .ReturnsAsync(storageAudit);

            // when
            ValueTask<Audit> modifyAuditTask =
                auditServiceMock.Object.ModifyAuditAsync(invalidAudit);

            AuditValidationException actualAuditValidationException =
                await Assert.ThrowsAsync<AuditValidationException>(
                    testCode: modifyAuditTask.AsTask);

            // then
            actualAuditValidationException.Should().BeEquivalentTo(expectedAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
               broker.GetCurrentUserAsync(),
                   Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAuditByIdAsync(invalidAudit.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAuditValidationException))),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
