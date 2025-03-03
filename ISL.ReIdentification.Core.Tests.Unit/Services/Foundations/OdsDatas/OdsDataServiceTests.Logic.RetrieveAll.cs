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
        public async Task ShouldReturnOdsDatas()
        {
            // given
            List<OdsData> randomOdsDatas = CreateRandomOdsDatas();
            IQueryable<OdsData> storageOdsDatas = randomOdsDatas.AsQueryable();
            IQueryable<OdsData> expectedOdsDatas = storageOdsDatas.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(storageOdsDatas);

            // when
            IQueryable<OdsData> actualOdsDatas =
                await this.odsDataService.RetrieveAllOdsDatasAsync();

            // then
            actualOdsDatas.Should().BeEquivalentTo(expectedOdsDatas);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}