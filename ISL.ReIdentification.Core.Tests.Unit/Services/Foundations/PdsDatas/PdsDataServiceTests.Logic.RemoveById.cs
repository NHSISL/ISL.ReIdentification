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
        public async Task ShouldRemovePdsDataByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputPdsDataId = randomId;
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData storagePdsData = randomPdsData;
            PdsData expectedInputPdsData = storagePdsData;
            PdsData deletedPdsData = expectedInputPdsData;
            PdsData expectedPdsData = deletedPdsData.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectPdsDataByIdAsync(inputPdsDataId))
                    .ReturnsAsync(storagePdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeletePdsDataAsync(expectedInputPdsData))
                    .ReturnsAsync(deletedPdsData);

            // when
            PdsData actualPdsData = await this.pdsDataService
                .RemovePdsDataByIdAsync(inputPdsDataId);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectPdsDataByIdAsync(inputPdsDataId),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeletePdsDataAsync(expectedInputPdsData),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}