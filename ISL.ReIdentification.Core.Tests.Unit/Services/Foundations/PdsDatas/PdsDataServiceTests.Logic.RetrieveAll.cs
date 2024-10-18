// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldReturnPdsDatas()
        {
            // given
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            IQueryable<PdsData> storagePdsDatas = randomPdsDatas.AsQueryable();
            IQueryable<PdsData> expectedPdsDatas = storagePdsDatas;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas);

            // when
            IQueryable<PdsData> actualPdsDatas =
                await this.pdsDataService.RetrieveAllPdsDatasAsync();

            // then
            actualPdsDatas.Should().BeEquivalentTo(expectedPdsDatas);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}