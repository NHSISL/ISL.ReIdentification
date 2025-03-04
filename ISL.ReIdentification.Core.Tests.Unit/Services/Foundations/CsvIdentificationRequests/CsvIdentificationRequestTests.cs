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
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly CsvIdentificationRequestService csvIdentificationRequestService;

        public CsvIdentificationRequestsTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.csvIdentificationRequestService = new CsvIdentificationRequestService(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest()
        {
            return CreateRandomCsvIdentificationRequest(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255));
        }

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest(
            DateTimeOffset dateTimeOffset,
            string userId) =>
            CreateCsvIdentificationRequestFiller(dateTimeOffset, userId).Create();

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest(
            DateTimeOffset dateTimeOffset) =>
            CreateCsvIdentificationRequestFiller(dateTimeOffset).Create();

        private static IQueryable<CsvIdentificationRequest> CreateRandomCsvIdentificationRequests()
        {
            return CreateCsvIdentificationRequestFiller(GetRandomDateTimeOffset(), GetRandomStringWithLengthOf(255))
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static CsvIdentificationRequest CreateRandomModifyCsvIdentificationRequest(
            DateTimeOffset dateTimeOffset,
            string userId)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(dateTimeOffset, userId);

            randomCsvIdentificationRequest.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomCsvIdentificationRequest;
        }

        private static CsvIdentificationRequest CreateRandomModifyCsvIdentificationRequest(
            DateTimeOffset dateTimeOffset)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(dateTimeOffset);

            randomCsvIdentificationRequest.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomCsvIdentificationRequest;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller(
            DateTimeOffset dateTimeOffset,
            string userId)
        {
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RequesterEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RecipientEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(userId)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(userId);

            return filler;
        }

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller(
            DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<CsvIdentificationRequest>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RequesterEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RecipientEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(user)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(user);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
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