// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Moq;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        private readonly Mock<IUserAccessService> userAccessServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserAccessProcessingService userAccessProcessingService;

        public UserAccessProcessingServiceTests()
        {
            this.userAccessServiceMock = new Mock<IUserAccessService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userAccessProcessingService = new UserAccessProcessingService(
                userAccessService: this.userAccessServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object
            );
        }

        private static UserAccess CreateRandomUserAccess() =>
            CreateUserAccessesFiller().Create();

        private static Filler<UserAccess> CreateUserAccessesFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }
    }
}
