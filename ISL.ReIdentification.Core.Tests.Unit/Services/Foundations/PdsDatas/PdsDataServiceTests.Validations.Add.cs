// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPdsDataIsNullAndLogItAsync()
        {
            // given
            PdsData nullPdsData = null;

            var nullPdsDataException =
                new NullPdsDataException(message: "PdsData is null.");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: nullPdsDataException);

            // when
            ValueTask<PdsData> addPdsDataTask =
                this.pdsDataService.AddPdsDataAsync(nullPdsData);

            PdsDataValidationException actualPdsDataValidationException =
                await Assert.ThrowsAsync<PdsDataValidationException>(
                    testCode: addPdsDataTask.AsTask);

            // then
            actualPdsDataValidationException.Should()
                .BeEquivalentTo(expectedPdsDataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsDataValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPdsDataIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidPdsData = new PdsData { 
                PseudoNhsNumber = invalidText,
                OrganisationName = invalidText,
                OrgCode = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var pdsDataServiceMock = new Mock<PdsDataService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            pdsDataServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidPdsData))
                    .ReturnsAsync(invalidPdsData);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidPdsDataException =
                new InvalidPdsDataException(
                    message: "Invalid pdsData. Please correct the errors and try again.");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.Id),
                values: "Id is invalid");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.PseudoNhsNumber),
                values: "Text is invalid");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.OrganisationName),
                values: "Text is invalid");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.OrgCode),
                values: "Text is invalid");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.CreatedDate),
                values:
                [
                    "Date is invalid",
                                $"Date is not recent. Expected a value between " +
                                    $"{startDate} and {endDate} but found {invalidPdsData.CreatedDate}"
                ]);

            invalidPdsDataException.AddData(
                key: nameof(PdsData.CreatedBy),
                values:
                [
                    "Text is invalid",
                                $"Expected value to be '{randomEntraUser.EntraUserId}' but found " +
                                    $"'{invalidPdsData.CreatedBy}'."
                ]);

            invalidPdsDataException.AddData(
                key: nameof(PdsData.UpdatedDate),
                values: "Date is invalid");

            invalidPdsDataException.AddData(
                key: nameof(PdsData.UpdatedBy),
                values: "Text is invalid");

            var expectedPdsDataValidationException =
                new PdsDataValidationException(
                    message: "PdsData validation error occurred, please fix errors and try again.",
                    innerException: invalidPdsDataException);

            // when
            ValueTask<PdsData> addPdsDataTask =
                pdsDataServiceMock.Object.AddPdsDataAsync(invalidPdsData);

            PdsDataValidationException actualPdsDataValidationException =
                await Assert.ThrowsAsync<PdsDataValidationException>(
                    testCode: addPdsDataTask.AsTask);

            // then
            actualPdsDataValidationException.Should()
                .BeEquivalentTo(expectedPdsDataValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertPdsDataAsync(It.IsAny<PdsData>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedPdsDataValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}