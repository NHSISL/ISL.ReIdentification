// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server.Tests.Integration.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.AccessAudit;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis.AccessAudits
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

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

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
            AccessAudit createdAccessAudit = await this.apiBroker.PostAccessAuditAsync(randomAccessAudit);

            return createdAccessAudit;
        }

        private async ValueTask<List<AccessAudit>> PostRandomAccessAuditsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomAccessAudits = new List<AccessAudit>();

            for (int i = 0; i < randomNumber; i++)
            {
                var audit = await PostRandomAccessAuditAsync();
                randomAccessAudits.Add(audit);
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
                .OnProperty(userAccess => userAccess.PseudoIdentifier).Use(GetRandomStringWithLengthOf(10))
                .OnProperty(userAccess => userAccess.Email).Use(GetRandomStringWithLengthOf(320))
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}