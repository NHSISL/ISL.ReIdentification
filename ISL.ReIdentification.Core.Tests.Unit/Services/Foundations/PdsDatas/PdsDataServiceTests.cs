// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IPdsDataService pdsDataService;

        public PdsDataServiceTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.pdsDataService = new PdsDataService(
                reIdentificationStorageBroker: this.reIdentificationStorageBroker.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static List<string> GetRandomStringsWithLengthOf(int length) =>
            Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(selector: _ => GetRandomStringWithLengthOf(length))
                .ToList();

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

        private static DateTimeOffset GetRandomFutureDateTimeOffset()
        {
            DateTime futureStartDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            int randomDaysInFuture = GetRandomNumber();
            DateTime futureEndDate = futureStartDate.AddDays(randomDaysInFuture).Date;

            return new DateTimeRange(earliestDate: futureStartDate, latestDate: futureEndDate).GetValue();
        }

        private static PdsData CreateRandomModifyPdsData(DateTimeOffset dateTimeOffset, string pdsId)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            PdsData randomPdsData = CreateRandomPdsData(dateTimeOffset, pdsId);

            randomPdsData.CreatedDate =
                randomPdsData.CreatedDate.AddDays(randomDaysInPast);

            return randomPdsData;
        }

        private static List<PdsData> CreateRandomPdsDatas()
        {
            return CreatePdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                pdsId: GetRandomStringWithLengthOf(255))
                .Create(count: GetRandomNumber())
                    .ToList();
        }

        private static PdsData CreateRandomPdsData() =>
            CreatePdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                pdsId: GetRandomStringWithLengthOf(255)).Create();

        private static PdsData CreateRandomPdsData(DateTimeOffset dateTimeOffset, string pdsId) =>
            CreatePdsDataFiller(dateTimeOffset, pdsId).Create();

        private static Filler<PdsData> CreatePdsDataFiller(DateTimeOffset dateTimeOffset, string pdsId)
        {
            //Hierarchy
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<PdsData>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(pdsData => pdsData.PseudoNhsNumber).Use(GetRandomStringWithLengthOf(10))
                .OnProperty(pdsData => pdsData.OrgCode).Use(GetRandomStringWithLengthOf(15))
                .OnProperty(odsData => odsData.CreatedBy).Use(pdsId)
                .OnProperty(odsData => odsData.UpdatedBy).Use(pdsId);

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