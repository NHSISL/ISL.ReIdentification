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
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        private readonly Mock<IUserAccessService> userAccessServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBroker;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserAccessProcessingService userAccessProcessingService;

        public UserAccessProcessingServiceTests()
        {
            this.userAccessServiceMock = new Mock<IUserAccessService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.identifierBroker = new Mock<IIdentifierBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userAccessProcessingService = new UserAccessProcessingService(
                userAccessService: this.userAccessServiceMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                identifierBroker: this.identifierBroker.Object,
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
            new IntRange(max: 15, min: 2).GetValue();

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
            Guid entraUserId) =>
            CreateUserAccessesFiller(dateTimeOffset, entraUserId).Create(GetRandomNumber()).ToList();

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
            Guid id = Guid.NewGuid();
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnProperty(userAccess => userAccess.Id).Use(id)
                .OnProperty(userAccess => userAccess.EntraUserId).Use(entraUserId)
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}
