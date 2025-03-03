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
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ILookupService lookupService;

        public LookupServiceTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.lookupService = new LookupService(
                reIdentificationStorageBroker: this.reIdentificationStorageBroker.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Lookup CreateRandomModifyLookup(DateTimeOffset dateTimeOffset, string userId)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Lookup randomLookup = CreateRandomLookup(dateTimeOffset, userId);

            randomLookup.CreatedDate =
                randomLookup.CreatedDate.AddDays(randomDaysInPast);

            return randomLookup;
        }

        private static IQueryable<Lookup> CreateRandomLookups()
        {
            return CreateLookupFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255))
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Lookup CreateRandomLookup()
        {
            return CreateLookupFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255)).Create();
        }

        private static Lookup CreateRandomLookup(DateTimeOffset dateTimeOffset, string userId) =>
            CreateLookupFiller(dateTimeOffset, userId).Create();

        private static Filler<Lookup> CreateLookupFiller(DateTimeOffset dateTimeOffset, string userId)
        {
            var filler = new Filler<Lookup>();
            string name = GetRandomStringWithLengthOf(220);
            string groupName = GetRandomStringWithLengthOf(220);

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(lookup => lookup.GroupName).Use(() => groupName)
                .OnProperty(lookup => lookup.Name).Use(() => name)
                .OnProperty(lookup => lookup.CreatedBy).Use(userId)
                .OnProperty(lookup => lookup.UpdatedBy).Use(userId);

            return filler;
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