// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnConvertCsvIdentificationRequestIfIdIsInvalidAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            invalidCsvIdentificationRequest.HasHeaderRecord = true;
            invalidCsvIdentificationRequest.IdentifierColumnIndex = 3;
            string csvIdentificationRequestData = GetRandomString();
            byte[] invalidCsvIdentificationRequestDataByteArray = Encoding.UTF8.GetBytes(csvIdentificationRequestData);
            invalidCsvIdentificationRequest.Data = invalidCsvIdentificationRequestDataByteArray;

            AccessRequest inputAccessRequest = new AccessRequest
            {
                CsvIdentificationRequest = invalidCsvIdentificationRequest
            };

            int randomNumber = GetRandomNumber();
            int csvIdentificationItemCount = randomNumber;
            int invalidIdentifierCount = GetRandomNumber(max: csvIdentificationItemCount, min: 1);

            IQueryable<CsvIdentificationItem> randomCsvIdentificationItems =
                CreateRandomCsvIdentificationItems(csvIdentificationItemCount);

            IQueryable<CsvIdentificationItem> outputCsvIdentificationItems = randomCsvIdentificationItems.DeepClone();
            int invalidIdentifierIndex = 0;

            foreach (var item in outputCsvIdentificationItems)
            {
                if (invalidIdentifierIndex < invalidIdentifierCount)
                {
                    item.Identifier = "";
                    invalidIdentifierIndex++;
                }
            }

            var expectedInvalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. " +
                        "Please correct the errors and try again.");

            for (int i = 0; i < invalidIdentifierCount; i++)
            {
                expectedInvalidIdentificationCoordinationException.UpsertDataList(
                key: nameof(CsvIdentificationItem.Identifier),
                value: $"Text is invalid at row {i + 2}");
            }

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapCsvToObjectAsync<CsvIdentificationItem>(
                    csvIdentificationRequestData,
                    invalidCsvIdentificationRequest.HasHeaderRecord,
                    It.IsAny<Dictionary<string, int>>()))
                        .ReturnsAsync(outputCsvIdentificationItems.ToList());

            // when
            ValueTask<AccessRequest> convertCsvIdentificationRequestTask =
                this.identificationCoordinationService
                    .ConvertCsvIdentificationRequestToIdentificationRequest(inputAccessRequest);

            InvalidIdentificationCoordinationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<InvalidIdentificationCoordinationException>(
                    testCode: convertCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedInvalidIdentificationCoordinationException);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.MapCsvToObjectAsync<CsvIdentificationItem>(
                    csvIdentificationRequestData,
                    invalidCsvIdentificationRequest.HasHeaderRecord,
                    It.IsAny<Dictionary<string, int>>()),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
