// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldExtractFromFilepath()
        {
            // given
            Guid randomGuid = Guid.NewGuid();
            Guid contextId = randomGuid;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string timestamp = randomDateTimeOffset.ToString("yyyyMMddHHmms");
            string contextIdString = contextId.ToString();
            string randomString = GetRandomString();
            string container = GetRandomString();
            string fileName = randomString;
            string fileExtension = ".csv";
            string filepath = $"/{container}/{contextIdString}/outbox/subdirectory/{fileName}{fileExtension}";
            string inputFilepath = filepath;
            string expectedLandingFilepath = $"outbox/subdirectory/{fileName}{fileExtension}";
            string expectedPickupFilepath = $"inbox/subdirectory/{fileName}_{timestamp}{fileExtension}";
            string expectedErrorFilepath = $"error/subdirectory/{fileName}_{timestamp}{fileExtension}";
            Guid expectedContextId = contextId;

            var expectedResult =
                (expectedLandingFilepath, expectedPickupFilepath, expectedErrorFilepath, expectedContextId);

            var service = new IdentificationCoordinationService(
                accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
                persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
                identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
                csvHelperBroker: this.csvHelperBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                projectStorageConfiguration: new ProjectStorageConfiguration
                {
                    Container = GetRandomString(),
                    LandingFolder = "outbox",
                    PickupFolder = "inbox",
                    ErrorFolder = "error"
                });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            var actualResult = await service
                .ExtractFromFilepath(inputFilepath);

            // then
            actualResult.Should().BeEquivalentTo(expectedResult);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
