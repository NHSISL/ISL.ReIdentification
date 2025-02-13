// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldAddPdsDataAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            PdsData randomPdsData = CreateRandomPdsData(
                randomDateTimeOffset,
                pdsId: randomEntraUser.EntraUserId);

            PdsData inputPdsData = randomPdsData;
            PdsData storagePdsData = inputPdsData;
            PdsData expectedPdsData = storagePdsData.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}