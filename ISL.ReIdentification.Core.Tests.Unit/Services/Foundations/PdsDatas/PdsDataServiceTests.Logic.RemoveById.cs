// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldRemovePdsDataByIdAsync()
        {
            // given
            long randomId = GetRandomNumber();
            long inputPdsDataRowId = randomId;
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData storagePdsData = randomPdsData;
            PdsData expectedInputPdsData = storagePdsData;
            PdsData deletedPdsData = expectedInputPdsData;
            PdsData expectedPdsData = deletedPdsData.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(inputPdsDataRowId))
                    .ReturnsAsync(storagePdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeletePdsDataAsync(expectedInputPdsData))
                    .ReturnsAsync(deletedPdsData);

            // when
            PdsData actualPdsData = await this.pdsDataService
                .RemovePdsDataByIdAsync(inputPdsDataRowId);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(inputPdsDataRowId),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeletePdsDataAsync(expectedInputPdsData),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}