// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldConvertCsvIdentificationRequestToIdentificationRequest()
        {
            // given
            Guid entraUserId = Guid.NewGuid();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest.RecipientEntraUserId = entraUserId;
            randomAccessRequest.ImpersonationContext = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            string csvDataString = Encoding.UTF8.GetString(inputAccessRequest.CsvIdentificationRequest.Data);
            bool hasHeaderRecord = inputAccessRequest.CsvIdentificationRequest.HasHeaderRecord;
            List<CsvIdentificationItem> randomCsvIdentificationItems = CreateRandomCsvIdentificationItems().ToList();
            List<IdentificationItem> convertedIdentificationItems = new List<IdentificationItem>();

            for (var index = 0; index < randomCsvIdentificationItems.Count; index++)
            {
                var identificationItem = new IdentificationItem
                {
                    HasAccess = false,
                    Identifier = randomCsvIdentificationItems[index].Identifier,
                    IsReidentified = false,
                    Message = String.Empty,
                    RowNumber = index.ToString()
                };

                convertedIdentificationItems.Add(identificationItem);
            }

            AccessRequest outputAccessRequest = randomAccessRequest.DeepClone();

            IdentificationRequest hydratedIdentificationRequest =
                HydrateAccessRequestIdentificationRequest(outputAccessRequest);

            outputAccessRequest.IdentificationRequest = hydratedIdentificationRequest;
            outputAccessRequest.IdentificationRequest.IdentificationItems = convertedIdentificationItems;

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(CsvIdentificationItem.Identifier),
                    randomAccessRequest.CsvIdentificationRequest.IdentifierColumnIndex }
            };

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapCsvToObjectAsync<CsvIdentificationItem>(csvDataString, hasHeaderRecord, fieldMappings))
                    .ReturnsAsync(randomCsvIdentificationItems);

            // when
            AccessRequest actualResult = await this.identificationCoordinationService
                .ConvertCsvIdentificationRequestToIdentificationRequest(inputAccessRequest);

            // then
            actualResult.Should().BeEquivalentTo(outputAccessRequest);

            this.csvHelperBrokerMock.Verify(service =>
                service.MapCsvToObjectAsync<CsvIdentificationItem>(csvDataString, hasHeaderRecord, fieldMappings),
                    Times.Once);

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
