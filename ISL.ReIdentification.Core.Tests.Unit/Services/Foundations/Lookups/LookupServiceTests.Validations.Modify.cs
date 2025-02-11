// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfLookupIsNullAndLogItAsync()
        {
            // given
            Lookup nullLookup = null;
            var nullLookupException = new NullLookupException(message: "Lookup is null.");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: nullLookupException);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(nullLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfLookupIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidLookup = new Lookup
            {
                GroupName = invalidText,
                Name = invalidText
            };

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.Id),
                values: "Id is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.GroupName),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.Name),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedDate),
                values: "Date is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values:
                new[] {
                    "Date is invalid",
                    $"Date is the same as {nameof(Lookup.CreatedDate)}"
                });

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: "Text is invalid");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            //then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfLookupHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup invalidLookup = CreateRandomModifyLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidLookup.GroupName = GetRandomStringWithLengthOf(221);
            invalidLookup.Name = GetRandomStringWithLengthOf(221);
            invalidLookup.CreatedBy = inputCreatedByUpdatedByString;
            invalidLookup.UpdatedBy = inputCreatedByUpdatedByString;

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.GroupName),
                values: $"Text exceed max length of {invalidLookup.GroupName.Length - 1} characters");

            invalidLookupException.AddData(
                key: nameof(Lookup.Name),
                values: $"Text exceed max length of {invalidLookup.Name.Length - 1} characters");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values: $"Text exceed max length of {invalidLookup.CreatedBy.Length - 1} characters");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: $"Text exceed max length of {invalidLookup.UpdatedBy.Length - 1} characters");

            var expectedLookupException = new
                LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should().BeEquivalentTo(expectedLookupException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup randomLookup = CreateRandomLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup invalidLookup = randomLookup;

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values: $"Date is the same as {nameof(Lookup.CreatedDate)}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id),
                    Times.Never);

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
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup randomLookup = CreateRandomLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup invalidLookup = randomLookup;
            invalidLookup.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomLookup.UpdatedDate}"
                ]);

            var expectedLookupValidationException = new LookupValidationException(
                message: "Lookup validation error occurred, please fix errors and try again.",
                innerException: invalidLookupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupVaildationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupVaildationException.Should().BeEquivalentTo(expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedLookupValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfLookupDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup randomLookup = CreateRandomModifyLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup nonExistLookup = randomLookup;
            Lookup nullLookup = null;

            var notFoundLookupException =
                new NotFoundLookupException(message: $"Lookup not found with Id: {nonExistLookup.Id}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: notFoundLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(nonExistLookup.Id))
                .ReturnsAsync(nullLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            // when 
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(nonExistLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(nonExistLookup.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup randomLookup = CreateRandomModifyLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup invalidLookup = randomLookup.DeepClone();
            Lookup storageLookup = invalidLookup.DeepClone();
            storageLookup.CreatedDate = storageLookup.CreatedDate.AddMinutes(randomMinutes);
            storageLookup.UpdatedDate = storageLookup.UpdatedDate.AddMinutes(randomMinutes);

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedDate),
                values: $"Date is not the same as {nameof(Lookup.CreatedDate)}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id))
                .ReturnsAsync(storageLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedLookupValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            Lookup randomLookup = CreateRandomModifyLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup invalidLookup = randomLookup.DeepClone();
            Lookup storageLookup = invalidLookup.DeepClone();
            invalidLookup.CreatedBy = Guid.NewGuid().ToString();
            storageLookup.UpdatedDate = storageLookup.CreatedDate;

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values: $"Text is not the same as {nameof(Lookup.CreatedBy)}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id))
                .ReturnsAsync(storageLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: modifyLookupTask.AsTask);

            // then
            actualLookupValidationException.Should().BeEquivalentTo(expectedLookupValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedLookupValidationException))),
                       Times.Once);

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
            Lookup randomLookup = CreateRandomModifyLookup(randomDateTimeOffset, userId: randomEntraUser.EntraUserId);
            Lookup invalidLookup = randomLookup;
            Lookup storageLookup = randomLookup.DeepClone();

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values: $"Date is the same as {nameof(Lookup.UpdatedDate)}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id))
                .ReturnsAsync(storageLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> modifyLookupTask =
                this.lookupService.ModifyLookupAsync(invalidLookup);

            // then
            await Assert.ThrowsAsync<LookupValidationException>(
                testCode: modifyLookupTask.AsTask);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectLookupByIdAsync(invalidLookup.Id),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}