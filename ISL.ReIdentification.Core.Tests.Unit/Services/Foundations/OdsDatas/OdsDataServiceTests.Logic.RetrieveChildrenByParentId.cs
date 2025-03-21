// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldRetrieveChildrenByParentIdAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = randomOdsData;
            List<OdsData> randomOdsDatas = CreateRandomOdsDataChildren(storageOdsData.OdsHierarchy);
            List<OdsData> childen = randomOdsDatas;
            List<OdsData> expectedOdsDatas = childen.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectOdsDataByIdAsync(inputOdsData.Id))
                    .ReturnsAsync(storageOdsData);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(childen.AsQueryable());

            // when
            List<OdsData> actualOdsDatas =
                await this.odsDataService.RetrieveChildrenByParentId(inputOdsData.Id);

            // then
            actualOdsDatas.Should().BeEquivalentTo(expectedOdsDatas);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectOdsDataByIdAsync(inputOdsData.Id),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}