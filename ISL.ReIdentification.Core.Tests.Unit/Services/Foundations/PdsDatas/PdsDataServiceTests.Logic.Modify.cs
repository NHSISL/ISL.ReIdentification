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
        public async Task ShouldModifyPdsDataAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PdsData randomPdsData = CreateRandomModifyPdsData(randomDateTimeOffset);
            PdsData inputPdsData = randomPdsData;
            PdsData storagePdsData = inputPdsData.DeepClone();
            PdsData updatedPdsData = inputPdsData;
            PdsData expectedPdsData = updatedPdsData.DeepClone();
            Guid pdsDataId = inputPdsData.Id;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(pdsDataId))
                    .ReturnsAsync(storagePdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdatePdsDataAsync(inputPdsData))
                    .ReturnsAsync(updatedPdsData);

            // when
            PdsData actualPdsData =
                await this.pdsDataService.ModifyPdsDataAsync(inputPdsData);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(inputPdsData.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdatePdsDataAsync(inputPdsData),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}