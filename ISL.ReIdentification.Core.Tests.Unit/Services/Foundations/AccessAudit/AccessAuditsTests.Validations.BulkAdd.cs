// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddAccessAuditAsync()
        {
            // given
            List<AccessAudit> nullAccessAudit = null;
            var nullAccessAuditException = new NullAccessAuditException(message: "Access audit list is null.");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: nullAccessAuditException);

            // when
            ValueTask addAccessAuditTask = this.accessAuditService.BulkAddAccessAuditAsync(nullAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))), Times.Once());

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfAccessAuditIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            List<AccessAudit> invalidAccessAudits = new List<AccessAudit> {
                new AccessAudit
                {
                    EntraUserId = invalidText,
                    PseudoIdentifier = invalidText,
                    Email = invalidText,
                    CreatedBy = invalidText,
                    UpdatedBy = invalidText,
                }
            };

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyBulkAddAuditAsync(invalidAccessAudits))
                    .ReturnsAsync(invalidAccessAudits);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

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
                values:
                [
                    "Date is invalid",
                    $"Date is not recent. Expected a value between " +
                        $"{startDate} and {endDate} but found {invalidAccessAudits.FirstOrDefault().CreatedDate}"
                ]);

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found " +
                        $"'{invalidAccessAudits.FirstOrDefault().CreatedBy}'."
                ]);

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: "Date is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: "Text is invalid");

            var exceptions = new List<Exception>();

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                exceptions.Add(invalidAccessAuditException);
            }

            AggregateException aggregateException = new AggregateException(
                    $"Unable to validate access for {exceptions.Count} audit access requests.",
                    exceptions);

            var failedServiceIdentificationRequestException =
                new FailedServiceAccessAuditException(
                    message: "Failed service access audit error occurred, contact support.",
                    innerException: aggregateException);

            var expectedAccessAuditServiceException =
                new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceIdentificationRequestException);

            // when
            ValueTask addAccessAuditTask =
                accessAuditServiceMock.Object.BulkAddAccessAuditAsync(invalidAccessAudits);

            AccessAuditServiceException actualAccessAuditServiceException =
                await Assert.ThrowsAsync<AccessAuditServiceException>(testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceException.Should()
                .BeEquivalentTo(expectedAccessAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionBulkOnAddIfAccessAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser(entraUserId: GetRandomStringWithLengthOf(256));

            List<AccessAudit> invalidAccessAudits = CreateRandomAccessAuditList(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                var inputCreatedByUpdatedByString = randomEntraUser.EntraUserId;
                invalidAccessAudit.EntraUserId = GetRandomStringWithLengthOf(256);
                invalidAccessAudit.Email = GetRandomStringWithLengthOf(321);
                invalidAccessAudit.PseudoIdentifier = GetRandomStringWithLengthOf(11);
                invalidAccessAudit.AuditType = GetRandomStringWithLengthOf(256);
                invalidAccessAudit.CreatedBy = inputCreatedByUpdatedByString;
                invalidAccessAudit.UpdatedBy = inputCreatedByUpdatedByString;
            }

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyBulkAddAuditAsync(invalidAccessAudits))
                    .ReturnsAsync(invalidAccessAudits);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().PseudoIdentifier.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.EntraUserId),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().EntraUserId.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().Email.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.AuditType),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().AuditType.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().CreatedBy.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text exceed max length of {invalidAccessAudits.FirstOrDefault().UpdatedBy.Length - 1} characters");

            var exceptions = new List<Exception>();

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                exceptions.Add(invalidAccessAuditException);
            }

            AggregateException aggregateException = new AggregateException(
                    $"Unable to validate access for {exceptions.Count} audit access requests.",
                    exceptions);

            var failedServiceIdentificationRequestException =
                new FailedServiceAccessAuditException(
                    message: "Failed service access audit error occurred, contact support.",
                    innerException: aggregateException);

            var expectedAccessAuditServiceException =
                new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceIdentificationRequestException);

            // when
            ValueTask addAccessAuditTask =
                accessAuditServiceMock.Object.BulkAddAccessAuditAsync(invalidAccessAudits);

            AccessAuditServiceException actualAccessAuditServiceException =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceException.Should()
                .BeEquivalentTo(expectedAccessAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            List<AccessAudit> invalidAccessAudits = CreateRandomAccessAuditList(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            string createdBy = GetRandomString();
            string updatedBy = GetRandomString();
            DateTimeOffset createdDate = GetRandomDateTimeOffset();
            DateTimeOffset updatedDate = GetRandomDateTimeOffset();

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                invalidAccessAudit.CreatedBy = createdBy;
                invalidAccessAudit.UpdatedBy = updatedBy;
                invalidAccessAudit.CreatedDate = createdDate;
                invalidAccessAudit.UpdatedDate = updatedDate;
            }

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyBulkAddAuditAsync(invalidAccessAudits))
                    .ReturnsAsync(invalidAccessAudits);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values:
                    $"Expected value to be '{randomEntraUser.EntraUserId}' " +
                    $"but found '{createdBy}'.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text is not the same as {nameof(AccessAudit.CreatedBy)}");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: $"Date is not the same as {nameof(AccessAudit.CreatedDate)}");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values:
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but " +
                        $"found {createdDate}");

            var exceptions = new List<Exception>();

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                exceptions.Add(invalidAccessAuditException);
            }

            AggregateException aggregateException = new AggregateException(
                    $"Unable to validate access for {exceptions.Count} audit access requests.",
                    exceptions);

            var failedServiceIdentificationRequestException =
                new FailedServiceAccessAuditException(
                    message: "Failed service access audit error occurred, contact support.",
                    innerException: aggregateException);

            var expectedAccessAuditServiceException =
                new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceIdentificationRequestException);

            // when
            ValueTask addAccessAuditTask =
                accessAuditServiceMock.Object.BulkAddAccessAuditAsync(invalidAccessAudits);

            AccessAuditServiceException actualAccessAuditServiceException =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceException.Should().BeEquivalentTo(
                expectedAccessAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            DateTimeOffset invalidDate =
                randomDateTimeOffset.AddSeconds(invalidSeconds);

            List<AccessAudit> invalidAccessAudits = CreateRandomAccessAuditList(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                invalidAccessAudit.CreatedDate = invalidDate;
                invalidAccessAudit.UpdatedDate = invalidDate;
            }

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBrokerMock.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyBulkAddAuditAsync(invalidAccessAudits))
                    .ReturnsAsync(invalidAccessAudits);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
            key: nameof(AccessAudit.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var exceptions = new List<Exception>();

            foreach (var invalidAccessAudit in invalidAccessAudits)
            {
                exceptions.Add(invalidAccessAuditException);
            }

            AggregateException aggregateException = new AggregateException(
                    $"Unable to validate access for {exceptions.Count} audit access requests.",
                    exceptions);

            var failedServiceIdentificationRequestException =
                new FailedServiceAccessAuditException(
                    message: "Failed service access audit error occurred, contact support.",
                    innerException: aggregateException);

            var expectedAccessAuditServiceException =
                new AccessAuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask addAccessAuditTask =
                accessAuditServiceMock.Object.BulkAddAccessAuditAsync(invalidAccessAudits);

            AccessAuditServiceException actualAccessAuditServiceException =
                await Assert.ThrowsAsync<AccessAuditServiceException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditServiceException.Should().BeEquivalentTo(
                expectedAccessAuditServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(invalidAccessAudits.Count));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditServiceException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
