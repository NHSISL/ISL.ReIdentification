// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        [Fact]
        public async Task ShouldRemoveOdsDataByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputOdsDataId = randomId;
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData storageOdsData = randomOdsData;
            OdsData expectedInputOdsData = storageOdsData;
            OdsData deletedOdsData = expectedInputOdsData;
            OdsData expectedOdsData = deletedOdsData.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(inputOdsDataId))
                    .ReturnsAsync(storageOdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteOdsDataAsync(expectedInputOdsData))
                    .ReturnsAsync(deletedOdsData);

            // when
            OdsData actualOdsData = await this.odsDataService
                .RemoveOdsDataByIdAsync(inputOdsDataId);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(inputOdsDataId),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteOdsDataAsync(expectedInputOdsData),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}