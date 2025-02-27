// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldReturnLookups()
        {
            // given
            IQueryable<Lookup> randomLookups = CreateRandomLookups();
            IQueryable<Lookup> storageLookups = randomLookups;
            IQueryable<Lookup> expectedLookups = storageLookups;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllLookupsAsync())
                    .ReturnsAsync(storageLookups);

            // when
            IQueryable<Lookup> actualLookups =
                await this.lookupService.RetrieveAllLookupsAsync();

            // then
            actualLookups.Should().BeEquivalentTo(expectedLookups);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllLookupsAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}