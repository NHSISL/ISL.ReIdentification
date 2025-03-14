// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Lookups;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class LookupsApiTests
    {
        [Fact]
        public async Task ShouldPostLookupAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            Lookup inputLookup = randomLookup;
            Lookup expectedLookup = inputLookup;

            // when 
            await this.apiBroker.PostLookupAsync(inputLookup);

            Lookup actualLookup =
                await this.apiBroker.GetLookupByIdAsync(inputLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }

        [Fact]
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

                actualLookup.Should().BeEquivalentTo(expectedLookup, options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate));

                await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
            }
        }

        [Fact]
        public async Task ShouldGetLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();
            Lookup expectedLookup = randomLookup;

            // when
            Lookup actualLookup = await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(expectedLookup, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }

        [Fact]
        public async Task ShouldPutLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();
            Lookup modifiedLookup = UpdateLookupWithRandomValues(randomLookup);

            // when
            await this.apiBroker.PutLookupAsync(modifiedLookup);
            Lookup actualLookup = await this.apiBroker.GetLookupByIdAsync(randomLookup.Id);

            // then
            actualLookup.Should().BeEquivalentTo(modifiedLookup, options => options
                .Excluding(property => property.CreatedBy)
                .Excluding(property => property.CreatedDate)
                .Excluding(property => property.UpdatedBy)
                .Excluding(property => property.UpdatedDate));

            await this.apiBroker.DeleteLookupByIdAsync(actualLookup.Id);
        }

        [Fact]
        public async Task ShouldDeleteLookupAsync()
        {
            // given
            Lookup randomLookup = await PostRandomLookupAsync();
            Lookup inputLookup = randomLookup;
            Lookup expectedLookup = inputLookup;

            // when
            Lookup deletedLookup =
                await this.apiBroker.DeleteLookupByIdAsync(inputLookup.Id);

            List<Lookup> actualResult =
                await this.apiBroker.GetSpecificLookupByIdAsync(inputLookup.Id);

            // then
            actualResult.Count().Should().Be(0);
        }
    }
}