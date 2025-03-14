// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Lookup;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.Lookups
{
    public partial class LookupApiTests
    {
        [Fact]
        public async Task ShouldGetAllLookupsAsync()
        {
            // given
            List<Lookup> randomLookups = await PostRandomLookupsAsync();
            List<Lookup> expectedLookups = randomLookups;

            // when
            List<Lookup> actualLookups = await this.apiBroker.GetAllLookupsAsync();

            // then
            actualLookups.Should().NotBeNull();

            foreach (Lookup expectedLookup in expectedLookups)
            {
                Lookup actualLookup = actualLookups
                    .Single(lookup => lookup.Id == expectedLookup.Id);

                actualLookup.Should().BeEquivalentTo(
                    expectedLookup,
                    options => options
                        .Excluding(property => property.CreatedBy)
                        .Excluding(property => property.CreatedDate)
                        .Excluding(property => property.UpdatedBy)
                        .Excluding(property => property.UpdatedDate));
            }

            // cleanup
            foreach (Lookup createdLookup in expectedLookups)
            {
                await this.apiBroker.DeleteLookupByIdAsync(createdLookup.Id);
            }
        }
    }
}