// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldAddPdsDataAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            PdsData randomPdsData = CreateRandomPdsData(randomDateTimeOffset);
            PdsData inputPdsData = randomPdsData;
            PdsData storagePdsData = inputPdsData;
            PdsData expectedPdsData = storagePdsData.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.InsertPdsDataAsync(inputPdsData))
                    .ReturnsAsync(storagePdsData);

            // when
            PdsData actualPdsData = await this.pdsDataService
                .AddPdsDataAsync(inputPdsData);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertPdsDataAsync(inputPdsData),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}