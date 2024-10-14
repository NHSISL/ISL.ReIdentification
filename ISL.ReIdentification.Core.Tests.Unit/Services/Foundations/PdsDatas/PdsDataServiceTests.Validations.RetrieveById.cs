// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            long invalidPdsDataId = default;

            var invalidPdsDataException =
                new InvalidPdsDataException(
                    message: "Invalid pdsData. Please correct the errors and try again.");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.RowId),
                values: "Id is invalid");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: invalidPdsDataException);

            // when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(invalidPdsDataId);

            PdsDataValidationException actualPdsDataValidationException =
                await Assert.ThrowsAsync<PdsDataValidationException>(
                    testCode: retrievePdsDataByIdTask.AsTask);

            // then
            actualPdsDataValidationException.Should()
                .BeEquivalentTo(expectedPdsDataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsDataValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPdsDataIsNotFoundAndLogItAsync()
        {
            //given
            long somePdsDataId = GetRandomNumber();
            PdsData noPdsData = null;
            var notFoundPdsDataException =
                new NotFoundPdsDataException(message: $"PdsData not found with Id: {somePdsDataId}");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: notFoundPdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync(noPdsData);

            //when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(somePdsDataId);

            PdsDataValidationException actualPdsDataValidationException =
                await Assert.ThrowsAsync<PdsDataValidationException>(
                    testCode: retrievePdsDataByIdTask.AsTask);

            //then
            actualPdsDataValidationException.Should().BeEquivalentTo(expectedPdsDataValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsDataValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}