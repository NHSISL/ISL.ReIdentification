// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly AccessAuditService accessAuditService;

        public AccessAuditTests()
        {
            this.reIdentificationStorageBrokerMock = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.accessAuditService = new AccessAuditService(
                reIdentificationStorageBroker: reIdentificationStorageBrokerMock.Object,
                dateTimeBroker: dateTimeBrokerMock.Object,
                securityBroker: securityBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<AccessAudit> CreateRandomAccessAudits()
        {
            return CreateAccessAuditFiller(GetRandomDateTimeOffset(), GetRandomStringWithLengthOf(255))
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static AccessAudit CreateRandomAccessAudit()
        {
            return CreateRandomAccessAudit(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255));
        }

        private List<AccessAudit> CreateRandomAccessAuditList(DateTimeOffset dateTimeOffset, string userId)
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(selector: number => CreateRandomAccessAudit(dateTimeOffset, userId))
                .ToList();
        }

        private static AccessAudit CreateRandomAccessAudit(DateTimeOffset dateTimeOffset, string userId) =>
            CreateAccessAuditFiller(dateTimeOffset, userId).Create();

        private static AccessAudit CreateRandomModifyAccessAudit(DateTimeOffset dateTimeOffset, string userId)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();
            AccessAudit randomAccessAudit = CreateRandomAccessAudit(dateTimeOffset, userId);
            randomAccessAudit.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomAccessAudit;
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Filler<AccessAudit> CreateAccessAuditFiller(DateTimeOffset dateTimeOffset, string userId)
        {
            var filler = new Filler<AccessAudit>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(userAccess => userAccess.PseudoIdentifier).Use(GetRandomStringWithLengthOf(10))
                .OnProperty(userAccess => userAccess.Email).Use(GetRandomStringWithLengthOf(320))
                .OnProperty(userAccess => userAccess.CreatedBy).Use(userId)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(userId);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private EntraUser CreateRandomEntraUser(string entraUserId = "")
        {
            var userId = string.IsNullOrWhiteSpace(entraUserId) ? GetRandomStringWithLengthOf(255) : entraUserId;

            return new EntraUser(
                entraUserId: userId,
                givenName: GetRandomString(),
                surname: GetRandomString(),
                displayName: GetRandomString(),
                email: GetRandomString(),
                jobTitle: GetRandomString(),
                roles: new List<string> { GetRandomString() },

                claims: new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(type: GetRandomString(), value: GetRandomString())
                });
        }
    }
}
