// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        [Fact]
        public async Task ShouldModifyOdsDataAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            OdsData randomOdsData = CreateRandomModifyOdsData(
                randomDateTimeOffset,
                odsId: randomEntraUser.EntraUserId);

            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = inputOdsData.DeepClone();
            OdsData updatedOdsData = inputOdsData;
            OdsData expectedOdsData = updatedOdsData.DeepClone();
            Guid odsDataId = inputOdsData.Id;

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(odsDataId))
                    .ReturnsAsync(storageOdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateOdsDataAsync(inputOdsData))
                    .ReturnsAsync(updatedOdsData);

            // when
            OdsData actualOdsData =
                await this.odsDataService.ModifyOdsDataAsync(inputOdsData);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(inputOdsData.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateOdsDataAsync(inputOdsData),
                    Times.Once);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}