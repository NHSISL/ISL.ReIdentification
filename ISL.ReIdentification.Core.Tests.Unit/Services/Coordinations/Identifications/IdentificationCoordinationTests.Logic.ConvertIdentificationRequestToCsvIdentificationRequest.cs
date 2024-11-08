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
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldConvertIdentificationRequestToCsvIdentificationRequest()
        {
            // given
            var identificationCoordinationServiceMock = new Mock<IdentificationCoordinationService>
               (this.accessOrchestrationServiceMock.Object,
               this.persistanceOrchestrationServiceMock.Object,
               this.identificationOrchestrationServiceMock.Object,
               this.csvHelperBrokerMock.Object,
               this.securityBrokerMock.Object,
               this.loggingBrokerMock.Object,
               this.dateTimeBrokerMock.Object,
               this.projectStorageConfiguration)
            { CallBase = true };

            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.CsvIdentificationRequest.IdentifierColumnIndex = 2;
            int identificationItemCount = randomAccessRequest.IdentificationRequest.IdentificationItems.Count;
            bool hasHeaderRecord = randomAccessRequest.CsvIdentificationRequest.HasHeaderRecord;
            string csvDataString = Encoding.UTF8.GetString(randomAccessRequest.CsvIdentificationRequest.Data);
            List<dynamic> mappedCsvObjects = CreateRandomDynamicObjects(identificationItemCount);
            List<dynamic> reIdentifiedItems = mappedCsvObjects.DeepClone();
            byte[] csvIdentificationRequestDataByteArray = Encoding.UTF8.GetBytes(CsvDataString());
            AccessRequest outputAccessRequest = randomAccessRequest.DeepClone();
            outputAccessRequest.CsvIdentificationRequest = new CsvIdentificationRequest();
            outputAccessRequest.CsvIdentificationRequest.Data = csvIdentificationRequestDataByteArray;
            outputAccessRequest.CsvIdentificationRequest.HasHeaderRecord = hasHeaderRecord;

            List<CsvIdentificationItem> csvIdentificationItems =
                CreateRandomCsvIdentificationItems(identificationItemCount).ToList();

            for (int i = 0; i < reIdentifiedItems.Count; i++)
            {
                reIdentifiedItems[i].Identifier = csvIdentificationItems[i].Identifier;
            }

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapCsvToObjectAsync<dynamic>(It.Is(SameStringAs(csvDataString)),
                    randomAccessRequest.CsvIdentificationRequest.HasHeaderRecord,
                    null))
                        .ReturnsAsync(mappedCsvObjects);

            identificationCoordinationServiceMock.Setup(service =>
                service.UpdateIdentifierColumnValues(It.Is(SameDynamicListAs(mappedCsvObjects)),
                    randomAccessRequest.IdentificationRequest.IdentificationItems,
                    randomAccessRequest.CsvIdentificationRequest.IdentifierColumnIndex))
                        .Returns(reIdentifiedItems);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapObjectToCsvAsync(
                    It.Is(SameDynamicListAs(reIdentifiedItems)),
                    randomAccessRequest.CsvIdentificationRequest.HasHeaderRecord,
                    null,
                    false))
                        .ReturnsAsync(CsvDataString());

            IdentificationCoordinationService identificationCoordinationService =
                identificationCoordinationServiceMock.Object;

            // when
            AccessRequest actualResult = await identificationCoordinationService
                .ConvertIdentificationRequestToCsvIdentificationRequest(randomAccessRequest);

            // then
            actualResult.Should().BeEquivalentTo(outputAccessRequest);

            this.csvHelperBrokerMock.Verify(service =>
                service.MapCsvToObjectAsync<dynamic>(It.Is(SameStringAs(csvDataString)),
                randomAccessRequest.CsvIdentificationRequest.HasHeaderRecord,
                null),
                    Times.Once);

            this.csvHelperBrokerMock.Verify(service =>
                service.MapObjectToCsvAsync(It.Is(SameDynamicListAs(reIdentifiedItems)),
                randomAccessRequest.CsvIdentificationRequest.HasHeaderRecord,
                null,
                false),
                    Times.Once);

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
