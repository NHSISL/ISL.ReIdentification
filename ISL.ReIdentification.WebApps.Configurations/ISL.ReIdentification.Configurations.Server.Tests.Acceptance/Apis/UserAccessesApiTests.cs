// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.UserAccesses;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class UserAccessesApiTests
    {
        private readonly ApiBroker apiBroker;

        public UserAccessesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<string> GetRandomStringsWithLengthOf(int length, int count = 0)
        {
            if (count == 0)
            {
                count = GetRandomNumber();
            }

            return Enumerable.Range(start: 0, count)
                .Select(selector: _ => GetRandomStringWithLengthOf(length))
                .ToList();
        }

        private static UserAccess CreateRandomUserAccess() =>
            CreateRandomUserAccessFiller().Create();

        private static BulkUserAccess CreateRandomBulkUserAccess()
        {
            return new BulkUserAccess
            {
                EntraUserId = GetRandomStringWithLengthOf(255),
                GivenName = GetRandomStringWithLengthOf(255),
                Surname = GetRandomStringWithLengthOf(255),
                DisplayName = GetRandomStringWithLengthOf(50),
                JobTitle = GetRandomStringWithLengthOf(50),
                Email = GetRandomStringWithLengthOf(320),
                UserPrincipalName = GetRandomStringWithLengthOf(50),
                OrgCodes = GetRandomStringsWithLengthOf(10)
            };
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Filler<UserAccess> CreateRandomUserAccessFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)

                .OnProperty(userAccess => userAccess.Email)
                    .Use(() => GetRandomStringWithLengthOf(320))

                .OnProperty(userAccess => userAccess.OrgCode)
                    .Use(() => GetRandomStringWithLengthOf(15))

                .OnProperty(userAccess => userAccess.CreatedDate).Use(now)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedDate).Use(now)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }

        private static UserAccess UpdateUserAccess(UserAccess inputUserAccess)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedUserAccess = CreateRandomUserAccess();
            updatedUserAccess.Id = inputUserAccess.Id;
            updatedUserAccess.CreatedDate = inputUserAccess.CreatedDate;
            updatedUserAccess.CreatedBy = inputUserAccess.CreatedBy;
            updatedUserAccess.UpdatedDate = now;

            return updatedUserAccess;
        }

        private async ValueTask<List<UserAccess>> PostRandomUserAccesses()
        {
            int randomNumber = GetRandomNumber();
            List<UserAccess> randomUserAccesses = new List<UserAccess>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomUserAccesses.Add(await this.PostRandomUserAccess());
            }

            return randomUserAccesses;
        }

        private async ValueTask<UserAccess> PostRandomUserAccess()
        {
            UserAccess randomUserAccess = CreateRandomUserAccess();

            return await this.apiBroker.PostUserAccessAsync(randomUserAccess);
        }

        private async ValueTask<BulkUserAccess> SetupBulkUserAccessesAsync(BulkUserAccess bulkUserAccess)
        {
            foreach (var orgCode in bulkUserAccess.OrgCodes)
            {
                var userId = Guid.NewGuid().ToString();
                var currentDateTime = DateTimeOffset.UtcNow;

                UserAccess randomUserAccess = new UserAccess
                {
                    Id = Guid.NewGuid(),
                    DisplayName = bulkUserAccess.DisplayName,
                    Email = bulkUserAccess.Email,
                    OrgCode = orgCode,
                    GivenName = bulkUserAccess.GivenName,
                    EntraUserId = bulkUserAccess.EntraUserId,
                    JobTitle = bulkUserAccess.JobTitle,
                    Surname = bulkUserAccess.Surname,
                    UserPrincipalName = bulkUserAccess.UserPrincipalName,
                    CreatedBy = userId,
                    CreatedDate = currentDateTime,
                    UpdatedBy = userId,
                    UpdatedDate = currentDateTime
                };

                await this.apiBroker.PostUserAccessAsync(randomUserAccess);
            }

            return bulkUserAccess;
        }
    }
}
