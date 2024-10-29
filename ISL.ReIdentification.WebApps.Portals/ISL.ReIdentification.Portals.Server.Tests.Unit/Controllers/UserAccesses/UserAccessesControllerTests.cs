// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using ISL.ReIdentification.Portals.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests : RESTFulController
    {
        private readonly Mock<IUserAccessProcessingService> userAccessProcessingServiceMock;
        private readonly UserAccessesController userAccessesController;

        public UserAccessesControllerTests()
        {
            this.userAccessProcessingServiceMock = new Mock<IUserAccessProcessingService>();
            this.userAccessesController = new UserAccessesController(userAccessProcessingServiceMock.Object);
        }

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

        private static BulkUserAccess CreateRandomBulkUserAccess()
        {
            return new BulkUserAccess
            {
                EntraUserId = Guid.NewGuid(),
                GivenName = GetRandomString(),
                Surname = GetRandomString(),
                DisplayName = GetRandomString(),
                JobTitle = GetRandomString(),
                Email = GetRandomString(),
                UserPrincipalName = GetRandomString(),
                OrgCodes = GetRandomStringsWithLengthOf(10)
            };
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new UserAccessProcessingValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAccessProcessingDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new UserAccessProcessingDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAccessProcessingServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserAccess CreateRandomUserAccess() =>
            CreateUserAccessesFiller().Create();

        private static IQueryable<UserAccess> CreateRandomUserAccesses()
        {
            return CreateUserAccessesFiller()
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(2, 10).GetValue();
        private static Filler<UserAccess> CreateUserAccessesFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}
