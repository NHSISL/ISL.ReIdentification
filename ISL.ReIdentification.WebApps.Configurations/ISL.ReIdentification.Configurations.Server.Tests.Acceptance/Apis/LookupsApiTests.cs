// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Lookups;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class LookupsApiTests
    {
        private readonly ApiBroker apiBroker;

        public LookupsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Lookup UpdateLookupWithRandomValues(Lookup inputLookup)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedLookup = CreateRandomLookup();
            updatedLookup.Id = inputLookup.Id;
            updatedLookup.CreatedDate = inputLookup.CreatedDate;
            updatedLookup.CreatedBy = inputLookup.CreatedBy;
            updatedLookup.UpdatedDate = now;

            return updatedLookup;
        }

        private async ValueTask<Lookup> PostRandomLookupAsync()
        {
            Lookup randomLookup = CreateRandomLookup();
            Lookup createdLookup = await this.apiBroker.PostLookupAsync(randomLookup);

            return createdLookup;
        }

        private async ValueTask<List<Lookup>> PostRandomLookupsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomLookups = new List<Lookup>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomLookups.Add(await PostRandomLookupAsync());
            }

            return randomLookups;
        }

        private static Lookup CreateRandomLookup() =>
            CreateRandomLookupFiller().Create();

        private static Filler<Lookup> CreateRandomLookupFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            string name = GetRandomStringWithLengthOf(220);
            string groupName = GetRandomStringWithLengthOf(220);
            var filler = new Filler<Lookup>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(lookup => lookup.GroupName).Use(() => groupName)
                .OnProperty(lookup => lookup.Name).Use(() => name)
                .OnProperty(lookup => lookup.CreatedDate).Use(now)
                .OnProperty(lookup => lookup.CreatedBy).Use(user)
                .OnProperty(lookup => lookup.UpdatedDate).Use(now)
                .OnProperty(lookup => lookup.UpdatedBy).Use(user);

            return filler;
        }
    }
}