// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
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
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidOdsData = new OdsData
            {
                OrganisationCode = invalidText,
                OrganisationName = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var odsDataServiceMock = new Mock<OdsDataService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            odsDataServiceMock.Setup(service =>
                service.ApplyModifyAuditAsync(invalidOdsData))
                    .ReturnsAsync(invalidOdsData);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidOdsDataException =
                new InvalidOdsDataException(
                    message: "Invalid odsData. Please correct the errors and try again.");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.Id),
                values: "Id is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.OrganisationCode),
                values: "Text is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.OrganisationName),
                values: "Text is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.CreatedDate),
                values: "Date is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.CreatedBy),
                values: "Text is invalid");

            invalidOdsDataException.AddData(
                key: nameof(OdsData.UpdatedBy),
                values:
                    [
                        "Text is invalid",
                        $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidText}'."
                    ]);

            invalidOdsDataException.AddData(
                key: nameof(OdsData.UpdatedDate),
                values:
                new[] {
                    "Date is invalid",
                    $"Date is the same as {nameof(OdsData.CreatedDate)}",
                    $"Date is not recent. Expected a value between {startDate} and {endDate} but found " +
                        $"{invalidOdsData.UpdatedDate}"
                });

            var expectedOdsDataValidationException =
               new OdsDataValidationException(
                   message: "OdsData validation error occurred, please fix errors and try again.",
                   innerException: invalidOdsDataException);

            // when
            ValueTask<OdsData> modifyOdsDataTask =
                odsDataServiceMock.Object.ModifyOdsDataAsync(invalidOdsData);

            OdsDataValidationException actualOdsDataValidationException =
                await Assert.ThrowsAsync<OdsDataValidationException>(
                    testCode: modifyOdsDataTask.AsTask);

            //then
            actualOdsDataValidationException.Should()
                .BeEquivalentTo(expectedOdsDataValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedOdsDataValidationException))),
                        Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(It.IsAny<OdsData>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfOdsDataDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            OdsData randomOdsData = CreateRandomModifyOdsData(
                randomDateTimeOffset,
                odsId: randomEntraUser.EntraUserId);

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