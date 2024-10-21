// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ISL.ReIdentification.Configurations.Server.Controllers;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests : RESTFulController
    {
        private readonly Mock<IUserAccessService> userAccessServiceMock;
        private readonly UserAccessesController userAccessesController;

        public UserAccessesControllerTests()
        {
            this.userAccessServiceMock = new Mock<IUserAccessService>();
            this.userAccessesController = new UserAccessesController(userAccessServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new UserAccessValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAccessDependencyValidationException(
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
                new UserAccessDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAccessServiceException(
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
