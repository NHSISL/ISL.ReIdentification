// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Lookups;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class LookupsApiTests
    {
        [Fact(Skip = "How do we do this if we don't expose a POST API")]
        public async Task ShouldGetAllLookupsAsync()
        {
            // given
            List<Lookup> randomLookups = await PostRandomLookupsAsync();
            List<Lookup> expectedLookups = randomLookups;

            // when
            List<Lookup> actualLookups = await this.apiBroker.GetAllLookupsAsync();

            // then
            foreach (Lookup expectedLookup in expectedLookups)
            {
                Lookup actualLookup = actualLookups.Single(approval => approval.Id == expectedLookup.Id);
                actualLookup.Should().BeEquivalentTo(expectedLookup);
                await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
            }
        }
    }
}