// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        private readonly Mock<IUserAccessService> userAccessServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserAccessProcessingService userAccessProcessingService;
        private readonly CompareLogic compareLogic;

        public UserAccessProcessingServiceTests()
        {
            this.userAccessServiceMock = new Mock<IUserAccessService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.userAccessProcessingService = new UserAccessProcessingService(
                userAccessService: this.userAccessServiceMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object
            );
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private Expression<Func<UserAccess, bool>> SameUserAccessAs(UserAccess expectedUserAccess)
        {
            return actualUserAccess =>
                this.compareLogic.Compare(expectedUserAccess, actualUserAccess)
                    .AreEqual;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static List<string> GetRandomStringsWithLengthOf(int length)
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(selector: _ => GetRandomStringWithLengthOf(length))
                .ToList();
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 10, min: 2).GetValue();

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new UserAccessValidationException(
                    message: "User access validation errors occurred, please try again.", innerException),

                new UserAccessDependencyValidationException(
                    message: "User access dependency validation occurred, please try again.", innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new UserAccessDependencyException(
                    message: "User access validation errors occurred, please try again.", innerException),

                new UserAccessServiceException(
                    message : "User access service error occurred, please contact support.", innerException)
            };
        }

        private static List<UserAccess> CreateRandomUserAccesses()
        {
            return CreateUserAccessesFiller(dateTimeOffset: GetRandomDateTimeOffset(), entraUserId: Guid.NewGuid())
                .Create(GetRandomNumber()).ToList();
        }

        private static List<UserAccess> CreateRandomUserAccesses(
            DateTimeOffset dateTimeOffset,
            Guid entraUserId,
            int? count = 0)
        {
            if (count == 0)
            {
                count = GetRandomNumber();
            }

            return CreateUserAccessesFiller(dateTimeOffset, entraUserId).Create(count.Value).ToList();
        }

        private static UserAccess CreateRandomUserAccess() =>
            CreateUserAccessesFiller(dateTimeOffset: GetRandomDateTimeOffset(), entraUserId: Guid.NewGuid()).Create();

        private static UserAccess CreateRandomUserAccess(
            DateTimeOffset dateTimeOffset,
            Guid entraUserId) =>
            CreateUserAccessesFiller(dateTimeOffset, entraUserId).Create();

        private static Filler<UserAccess> CreateUserAccessesFiller(
            DateTimeOffset dateTimeOffset,
            Guid entraUserId)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnProperty(userAccess => userAccess.Id).Use(Guid.NewGuid)
                .OnProperty(userAccess => userAccess.EntraUserId).Use(entraUserId)
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}
