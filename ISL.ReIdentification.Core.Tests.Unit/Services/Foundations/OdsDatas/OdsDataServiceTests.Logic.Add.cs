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
        public async Task ShouldAddOdsDataAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            
            OdsData randomOdsData = CreateRandomOdsData(
                randomDateTimeOffset, 
                odsId: randomEntraUser.EntraUserId);
            
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = inputOdsData;
            OdsData expectedOdsData = storageOdsData.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.InsertOdsDataAsync(inputOdsData))
                    .ReturnsAsync(storageOdsData);

            // when
            OdsData actualOdsData = await this.odsDataService
                .AddOdsDataAsync(inputOdsData);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertOdsDataAsync(inputOdsData),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}