// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdWhenCsvIdentificationRequestIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidCsvIdentificationRequestId = Guid.Empty;

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid csv identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CsvIdentificationRequest validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> removeCsvIdentificationRequestByIdTask =
                this.csvIdentificationRequestService.RemoveCsvIdentificationRequestByIdAsync(invalidCsvIdentificationRequestId);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: removeCsvIdentificationRequestByIdTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfCsvIdentificationRequestIdNotFoundAndLogitAsync()
        {
            // given
            var someCsvIdentificationRequestId = Guid.NewGuid();
            CsvIdentificationRequest nullCsvIdentificationRequest = null;
            var innerException = new Exception();

            var notFoundCsvIdentificationRequestException =
                new NotFoundCsvIdentificationRequestException(
                    message: $"CsvIdentificationRequest not found with id: {someCsvIdentificationRequestId}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CsvIdentificationRequest validation error occurred, please fix errors and try again.",
                    innerException: notFoundCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ReturnsAsync(nullCsvIdentificationRequest);

            // when
            ValueTask<CsvIdentificationRequest> removeCsvIdentificationRequestByIdTask =
                this.csvIdentificationRequestService.RemoveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: removeCsvIdentificationRequestByIdTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
