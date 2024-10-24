// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfOdsDataIsNullAndLogItAsync()
        {
            // given
            OdsData nullOdsData = null;
            var nullOdsDataException = new NullOdsDataException(message: "OdsData is null.");

            var expectedOdsDataValidationException =
                new OdsDataValidationException(
                    message: "OdsData validation error occurred, please fix errors and try again.",
                    innerException: nullOdsDataException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(nullOdsData);

            OdsDataValidationException actualOdsDataValidationException =
                await Assert.ThrowsAsync<OdsDataValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataValidationException.Should()
                .BeEquivalentTo(expectedOdsDataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfOdsDataIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidOdsData = new OdsData
            {
                OrganisationCode = invalidText
            };

            var invalidOdsDataException =
                new InvalidOdsDataException(
                    message: "Invalid odsData. Please correct the errors and try again.");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.Id),
                values: "Id is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.OrganisationCode),
                values: "Text is invalid");

            var expectedOdsDataValidationException =
                new OdsDataValidationException(
                    message: "OdsData validation error occurred, please fix errors and try again.",
                    innerException: invalidOdsDataException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(invalidOdsData);

            OdsDataValidationException actualOdsDataValidationException =
                await Assert.ThrowsAsync<OdsDataValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            //then
            actualOdsDataValidationException.Should()
                .BeEquivalentTo(expectedOdsDataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataValidationException))),
                        Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfOdsDataDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            OdsData randomOdsData = CreateRandomModifyOdsData(randomDateTimeOffset);
            OdsData nonExistOdsData = randomOdsData;
            OdsData nullOdsData = null;

            var notFoundOdsDataException =
                new NotFoundOdsDataException(message: $"OdsData not found with Id: {nonExistOdsData.Id}");

            var expectedOdsDataValidationException =
                new OdsDataValidationException(
                    message: "OdsData validation error occurred, please fix errors and try again.",
                    innerException: notFoundOdsDataException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(nonExistOdsData.Id))
                .ReturnsAsync(nullOdsData);

            // when 
            ValueTask<OdsData> modifyOdsDataTask =
                this.odsDataService.ModifyOdsDataAsync(nonExistOdsData);

            OdsDataValidationException actualOdsDataValidationException =
                await Assert.ThrowsAsync<OdsDataValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            // then
            actualOdsDataValidationException.Should()
                .BeEquivalentTo(expectedOdsDataValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(nonExistOdsData.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}