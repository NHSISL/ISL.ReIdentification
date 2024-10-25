// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCsvIdentificationRequestIsNullAndLogItAsync()
        {
            //given
            CsvIdentificationRequest nullCsvIdentificationRequest = null;

            var nullCsvIdentificationRequestException =
                new NullCsvIdentificationRequestException(message: "CSV identification request is null.");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: nullCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(nullCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCsvIdentificationRequestIsInvalidAndLogItAsync(
            string invalidText)
        {
            //given
            var invalidCsvIdentificationRequest = new CsvIdentificationRequest
            {
                RequesterEmail = invalidText,
                RecipientEmail = invalidText,
                IdentifierColumn = invalidText
            };

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEntraUserId),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEntraUserId),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.IdentifierColumn),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedDate),
                values: "Date is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(CsvIdentificationRequest.CreatedDate)}"
                    });

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(
                    invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnModifyIfCsvIdentificationRequestHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(dateTimeOffset: randomDateTimeOffset);

            var username = GetRandomStringWithLengthOf(256);
            invalidCsvIdentificationRequest.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.RecipientEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.IdentifierColumn = GetRandomStringWithLengthOf(11);
            invalidCsvIdentificationRequest.CreatedBy = username;
            invalidCsvIdentificationRequest.UpdatedBy = username;

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEmail),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.RequesterEmail.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEmail),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.RecipientEmail.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.IdentifierColumn),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.IdentifierColumn.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values: $"Text exceed max length of {invalidCsvIdentificationRequest.CreatedBy.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: $"Text exceed max length of {invalidCsvIdentificationRequest.UpdatedBy.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: $"Date is the same as {nameof(CsvIdentificationRequest.CreatedDate)}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService
                    .ModifyCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset);

            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: $"Date is the same as {nameof(CsvIdentificationRequest.CreatedDate)}");

            var expectedCsvIdentificationRequestValidationException = new CsvIdentificationRequestValidationException(
                message: "CSV identification request validation error occurred, please fix errors and try again.",
                innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService
                    .ModifyCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

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
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset);

            randomCsvIdentificationRequest.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but " +
                    $"found {randomCsvIdentificationRequest.UpdatedDate}"
                ]);

            var expectedCsvIdentificationRequestValidationException = new CsvIdentificationRequestValidationException(
                message: "CSV identification request validation error occurred, please fix errors and try again.",
                innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(randomCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCsvIdentificationRequestDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegative = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset);

            CsvIdentificationRequest nonExistingCsvIdentificationRequest = randomCsvIdentificationRequest;
            nonExistingCsvIdentificationRequest.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            CsvIdentificationRequest nullCsvIdentificationRequest = null;

            var notFoundCsvIdentificationRequestException =
                new NotFoundCsvIdentificationRequestException(
                    message: $"CSV identification request not found with id: {nonExistingCsvIdentificationRequest.Id}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: notFoundCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(nonExistingCsvIdentificationRequest.Id))
                    .ReturnsAsync(nullCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(nonExistingCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(nonExistingCsvIdentificationRequest.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomModifyCsvIdentificationRequest(randomDateTimeOffset);

            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest storedCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            storedCsvIdentificationRequest.CreatedBy = GetRandomString();

            storedCsvIdentificationRequest.CreatedDate =
                storedCsvIdentificationRequest.CreatedDate.AddMinutes(randomMinutes);

            storedCsvIdentificationRequest.UpdatedDate =
                storedCsvIdentificationRequest.UpdatedDate.AddMinutes(randomMinutes);

            Guid CsvIdentificationRequestId = invalidCsvIdentificationRequest.Id;

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values: $"Text is not the same as {nameof(CsvIdentificationRequest.CreatedBy)}");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedDate),
                values: $"Date is not the same as {nameof(CsvIdentificationRequest.CreatedDate)}");

            var expectedCsvIdentificationRequestValidationException = new CsvIdentificationRequestValidationException(
                message: "CSV identification request validation error occurred, please fix errors and try again.",
                innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(CsvIdentificationRequestId))
                    .ReturnsAsync(storedCsvIdentificationRequest);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(
                    invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(invalidCsvIdentificationRequest.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
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

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomModifyCsvIdentificationRequest(randomDateTimeOffset);

            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;

            CsvIdentificationRequest storageCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            invalidCsvIdentificationRequest.UpdatedDate = storageCsvIdentificationRequest.UpdatedDate;

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: $"Date is the same as {nameof(CsvIdentificationRequest.UpdatedDate)}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(invalidCsvIdentificationRequest.Id))
                .ReturnsAsync(storageCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> modifyCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
               await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                   testCode: modifyCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(invalidCsvIdentificationRequest.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
