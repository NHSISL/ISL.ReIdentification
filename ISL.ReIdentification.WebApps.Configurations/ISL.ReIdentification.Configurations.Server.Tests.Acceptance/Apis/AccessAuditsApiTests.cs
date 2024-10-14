// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.AccessAudit;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class AccessAuditsApiTests
    {
        private readonly ApiBroker apiBroker;

        public AccessAuditsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static AccessAudit CreateRandomAccessAudit() =>
           CreateRandomAccessAuditFiller().Create();

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static AccessAudit UpdateAccessAuditWithRandomValues(AccessAudit inputAccessAudit)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedAccessAudit = CreateRandomAccessAudit();
            updatedAccessAudit.Id = inputAccessAudit.Id;
            updatedAccessAudit.CreatedDate = inputAccessAudit.CreatedDate;
            updatedAccessAudit.CreatedBy = inputAccessAudit.CreatedBy;
            updatedAccessAudit.UpdatedDate = now;

            return updatedAccessAudit;
        }

        private async ValueTask<AccessAudit> PostRandomAccessAuditAsync()
        {
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            await this.apiBroker.PostAccessAuditAsync(randomAccessAudit);

            return randomAccessAudit;
        }

        private async ValueTask<List<AccessAudit>> PostRandomAccessAuditsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomAccessAudits = new List<AccessAudit>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomAccessAudits.Add(await PostRandomAccessAuditAsync());
            }

            return randomAccessAudits;
        }

        private static Filler<AccessAudit> CreateRandomAccessAuditFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<AccessAudit>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)
                .OnProperty(userAccess => userAccess.PseudoIdentifier).Use(GetRandomStringWithLengthOf(9))
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}
