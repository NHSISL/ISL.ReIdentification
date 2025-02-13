// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configuration.Server.Tests.Integration.Models;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class LookupApiTests
    {
        private readonly ApiBroker apiBroker;

        public LookupApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static DateTimeOffset GetRandomPastDateTimeOffset()
        {
            DateTime now = DateTimeOffset.UtcNow.Date;
            int randomDaysInPast = GetRandomNegativeNumber();
            DateTime pastDateTime = now.AddDays(randomDaysInPast).Date;

            return new DateTimeRange(earliestDate: pastDateTime, latestDate: now).GetValue();
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GetRandomEmail()
        {
            string randomPrefix = GetRandomStringWithLengthOf(15);
            string emailSuffix = "@email.com";

            return randomPrefix + emailSuffix;
        }

        private static Lookup CreateRandomLookup() =>
            CreateRandomLookupFiller().Create();

        private static Filler<Lookup> CreateRandomLookupFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Lookup>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(lookup => lookup.GroupName).Use(() => GetRandomStringWithLengthOf(220))
                .OnProperty(lookup => lookup.Name).Use(() => GetRandomStringWithLengthOf(220))
                .OnProperty(lookup => lookup.CreatedDate).Use(now)
                .OnProperty(lookup => lookup.CreatedBy).Use(user)
                .OnProperty(lookup => lookup.UpdatedDate).Use(now)
                .OnProperty(lookup => lookup.UpdatedBy).Use(user);

            return filler;
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
            createdLookup.Should().BeEquivalentTo(randomLookup);

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
    }
}