// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using ISL.ReIdentification.Portals.Server.Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAgreements
{
    public partial class UserAgreementsControllerTests : RESTFulController
    {
        private readonly Mock<IUserAgreementService> userAgreementServiceMock;
        private readonly UserAgreementsController userAgreementsController;

        public UserAgreementsControllerTests()
        {
            this.userAgreementServiceMock = new Mock<IUserAgreementService>();
            this.userAgreementsController = new UserAgreementsController(userAgreementServiceMock.Object);
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

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new UserAgreementValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAgreementDependencyValidationException(
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
                new UserAgreementDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserAgreementServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserAgreement CreateRandomUserAgreement() =>
            CreateUserAgreementsFiller().Create();

        private static IQueryable<UserAgreement> CreateRandomUserAgreements()
        {
            return CreateUserAgreementsFiller()
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(2, 10).GetValue();
        private static Filler<UserAgreement> CreateUserAgreementsFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<UserAgreement>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(userAgreement => userAgreement.CreatedBy).Use(user)
                .OnProperty(userAgreement => userAgreement.UpdatedBy).Use(user);

            return filler;
        }
    }
}
