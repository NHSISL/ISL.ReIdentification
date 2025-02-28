// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldCreateAccessRequestWithCsvIdentificationRequestAsync()
        {
            // given
            Guid randomImpersonationContextId = Guid.NewGuid();
            Guid inputImpersonationContextId = randomImpersonationContextId;
            string inputContainer = randomImpersonationContextId.ToString();
            string inputFilepath = GetRandomString();
            string pseudoIdentifier = "0000000001";
            string randomHeaderValue = GetRandomStringWithLengthOf(10);
            string randomIdentifierHeaderValue = GetRandomStringWithLengthOf(10);
            string randomValue = GetRandomStringWithLengthOf(10);
            StringBuilder retrievedCsv = new StringBuilder();

            retrievedCsv.AppendLine(
                $"{randomHeaderValue}0,{randomHeaderValue}1,{randomIdentifierHeaderValue}");

            retrievedCsv.AppendLine($"{randomValue},{randomValue},{pseudoIdentifier}");
            string retrievedCsvString = retrievedCsv.ToString();
            byte[] retrievedCsvData = Encoding.UTF8.GetBytes(retrievedCsvString);
            MemoryStream randomStream = new MemoryStream();
            MemoryStream returnedStream = new MemoryStream(retrievedCsvData);
            MemoryStream outputStream = randomStream;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            AccessRequest retrievedAccessRequest = randomAccessRequest;
            retrievedAccessRequest.ImpersonationContext.HasHeaderRecord = true;
            retrievedAccessRequest.ImpersonationContext.IdentifierColumn = randomIdentifierHeaderValue;

            AccessRequest expectedAccessRequest =
                ConvertImpersonationContextToCsvIdentificationRequest(retrievedAccessRequest);

            expectedAccessRequest.CsvIdentificationRequest.Data = retrievedCsvData;

            this.identificationOrchestrationServiceMock
                .Setup(service => service
                    .RetrieveDocumentByFileNameAsync(outputStream, inputFilepath, inputContainer))
                        .Callback<Stream, string, string>((output, fileName, container) =>
                        {
                            returnedStream.Position = 0;
                            returnedStream.CopyTo(output);
                        })
                        .Returns(ValueTask.CompletedTask);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(retrievedAccessRequest);

            var service = new IdentificationCoordinationService(
                accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
                persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
                identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
                csvHelperBroker: this.csvHelperBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                projectStorageConfiguration: this.projectStorageConfiguration);

            // when
            var actualAccessRequest = await service
                .CreateAccessRequestWithCsvIdentificationRequestAsync(inputContainer, inputFilepath);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
                    Times.Once);

            this.identificationOrchestrationServiceMock.Verify(service =>
                service.RetrieveDocumentByFileNameAsync(
                    It.Is(SameStreamAs(outputStream)),
                    inputFilepath,
                    inputContainer),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
