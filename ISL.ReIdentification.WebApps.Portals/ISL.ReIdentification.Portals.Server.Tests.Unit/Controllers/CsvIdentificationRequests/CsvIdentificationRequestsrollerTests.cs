// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ISL.ReIdentification.Portals.Server.Controllers;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests : RESTFulController
    {
        private readonly Mock<ICsvIdentificationRequestService> csvIdentificationRequestServiceMock;
        private readonly CsvIdentificationRequestsController csvIdentificationRequestsController;

        public CsvIdentificationRequestsControllerTests()
        {
            csvIdentificationRequestServiceMock = new Mock<ICsvIdentificationRequestService>();

            csvIdentificationRequestsController = new CsvIdentificationRequestsController(
                csvIdentificationRequestServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new CsvIdentificationRequestValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new CsvIdentificationRequestDependencyValidationException(
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
                new CsvIdentificationRequestDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new CsvIdentificationRequestServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateCsvIdentificationRequestFiller().Create();

        private static IQueryable<CsvIdentificationRequest> CreateRandomCsvIdentificationRequests()
        {
            return CreateCsvIdentificationRequestFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);

            return filler;
        }
    }
}
