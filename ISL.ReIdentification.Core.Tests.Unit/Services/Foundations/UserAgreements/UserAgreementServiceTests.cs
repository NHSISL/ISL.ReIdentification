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
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserAgreementService userAgreementService;

        public UserAgreementServiceTests()
        {
            this.reIdentificationStorageBrokerMock = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userAgreementService = new UserAgreementService(
                reIdentificationStorageBroker: this.reIdentificationStorageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserAgreement CreateRandomModifyUserAgreement(DateTimeOffset dateTimeOffset, string userId)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            UserAgreement randomUserAgreement = CreateRandomUserAgreement(dateTimeOffset, userId);

            randomUserAgreement.CreatedDate =
                randomUserAgreement.CreatedDate.AddDays(randomDaysInPast);

            return randomUserAgreement;
        }

        private static IQueryable<UserAgreement> CreateRandomUserAgreements()
        {
            return CreateUserAgreementFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255))
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static UserAgreement CreateRandomUserAgreement()
        {
            return CreateUserAgreementFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                userId: GetRandomStringWithLengthOf(255))
                    .Create();
        }

        private static UserAgreement CreateRandomUserAgreement(DateTimeOffset dateTimeOffset, string userId) =>
            CreateUserAgreementFiller(dateTimeOffset, userId).Create();

        private static Filler<UserAgreement> CreateUserAgreementFiller(DateTimeOffset dateTimeOffset, string userId)
        {
            var filler = new Filler<UserAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(userAgreement => userAgreement.CreatedBy).Use(userId)
                .OnProperty(userAgreement => userAgreement.UpdatedBy).Use(userId);

            return filler;
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
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