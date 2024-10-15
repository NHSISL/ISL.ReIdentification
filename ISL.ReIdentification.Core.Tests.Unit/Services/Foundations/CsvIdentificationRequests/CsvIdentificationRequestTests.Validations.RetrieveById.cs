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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdWhenCsvIdentificationRequestIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidCsvIdentificationRequestId = Guid.Empty;

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            var expectedCsvIdentificationRequestValidationException =
            new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> retrieveCsvIdentificationRequestByIdTask =
                this.csvIdentificationRequestService.RetrieveCsvIdentificationRequestByIdAsync(
                    invalidCsvIdentificationRequestId);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: retrieveCsvIdentificationRequestByIdTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfCsvIdentificationRequestIdNotFoundAndLogitAsync()
        {
            //given
            var someCsvIdentificationRequestId = Guid.NewGuid();
            CsvIdentificationRequest nullCsvIdentificationRequest = null;
            var innerException = new Exception();

            var notFoundCsvIdentificationRequestException =
                new NotFoundCsvIdentificationRequestException(
                    message: $"CSV identification request not found with id: {someCsvIdentificationRequestId}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: notFoundCsvIdentificationRequestException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ReturnsAsync(nullCsvIdentificationRequest);

            // when
            ValueTask<CsvIdentificationRequest> retrieveCsvIdentificationRequestByIdTask =
                this.csvIdentificationRequestService.RetrieveCsvIdentificationRequestByIdAsync(
                    someCsvIdentificationRequestId);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: retrieveCsvIdentificationRequestByIdTask.AsTask);

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
