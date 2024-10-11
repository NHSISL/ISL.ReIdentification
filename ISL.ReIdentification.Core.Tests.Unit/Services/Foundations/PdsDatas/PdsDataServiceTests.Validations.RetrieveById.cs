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
            var invalidPdsDataRowId = default(long);

            var invalidPdsDataException =
                new InvalidPdsDataException(
                    message: "Invalid pds data. Please correct the errors and try again.");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.RowId),
                values: "Id is invalid");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: invalidPdsDataException);

            // when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(invalidPdsDataRowId);

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

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPdsDataIsNotFoundAndLogItAsync()
        {
            //given
            long somePdsDataRowId = GetRandomNumber();
            PdsData noPdsData = null;

            var notFoundPdsDataException =
                new NotFoundPdsDataException(message: $"PDS data not found with Id: {somePdsDataRowId}");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: notFoundPdsDataException);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()))
                    .ReturnsAsync(noPdsData);

            //when
            ValueTask<PdsData> retrievePdsDataByIdTask =
                this.pdsDataService.RetrievePdsDataByIdAsync(somePdsDataRowId);

            PdsDataValidationException actualPdsDataValidationException =
                await Assert.ThrowsAsync<PdsDataValidationException>(
                    testCode: retrievePdsDataByIdTask.AsTask);

            //then
            actualPdsDataValidationException.Should().BeEquivalentTo(expectedPdsDataValidationException);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectPdsDataByIdAsync(It.IsAny<long>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsDataValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
