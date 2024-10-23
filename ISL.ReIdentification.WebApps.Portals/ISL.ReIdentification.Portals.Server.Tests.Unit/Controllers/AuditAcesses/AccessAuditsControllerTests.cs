// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Portals.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests : RESTFulController
    {

        private readonly Mock<IAccessAuditService> accessAuditServiceMock;
        private readonly AccessAuditsController accessAuditsController;

        public AccessAuditsControllerTests()
        {
            accessAuditServiceMock = new Mock<IAccessAuditService>();
            accessAuditsController = new AccessAuditsController(accessAuditServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new AccessAuditValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new AccessAuditDependencyValidationException(
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
                new AccessAuditDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new AccessAuditServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static AccessAudit CreateRandomAccessAudit() =>
            CreateAccessAuditFiller().Create();

        private static IQueryable<AccessAudit> CreateRandomAccessAudits()
        {
            return CreateAccessAuditFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<AccessAudit> CreateAccessAuditFiller()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<AccessAudit>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset);

            return filler;
        }
    }
}