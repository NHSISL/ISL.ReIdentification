// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using ISL.ReIdentification.Configurations.Server.Controllers;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests : RESTFulController
    {
        private readonly Mock<IIdentificationCoordinationService> identificationCoordinationServiceMock;
        private readonly ReIdentificationController reIdentificationController;

        public ReIdentificationControllerTests()
        {
            this.identificationCoordinationServiceMock = new Mock<IIdentificationCoordinationService>();
            this.reIdentificationController =
                new ReIdentificationController(this.identificationCoordinationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new IdentificationCoordinationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new IdentificationCoordinationDependencyValidationException(
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
                new IdentificationCoordinationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new IdentificationCoordinationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller().Create();

        private static Filler<AccessRequest> CreateAccessRequestFiller()
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnProperty(request => request.ImpersonationContextRequest).Use(CreateRandomImpersonationContext)

                .OnProperty(request => request.IdentificationRequest)
                    .Use(CreateRandomIdentificationRequest());

            return filler;
        }

        private static IdentificationRequest CreateRandomIdentificationRequest() =>
            CreateIdentificationRequestFiller().Create();

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller()
        {
            var filler = new Filler<IdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.IdentificationItems)
                    .Use(CreateRandomIdentificationItems(GetRandomNumber()));

            return filler;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateRandomImpersonationContext(dateTimeOffset: GetRandomDateTimeOffset());

        private static ImpersonationContext CreateRandomImpersonationContext(DateTimeOffset dateTimeOffset) =>
            CreateImpersonationContextsFiller(dateTimeOffset).Create();

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset);

            return filler;
        }

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller(int count)
        {
            var filler = new Filler<IdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.IdentificationItems).Use(CreateRandomIdentificationItems(count));

            return filler;
        }

        private static List<IdentificationItem> CreateRandomIdentificationItems(int count)
        {
            return CreateIdentificationItemFiller()
                .Create(count)
                    .ToList();
        }

        private static Filler<IdentificationItem> CreateIdentificationItemFiller() =>
            new Filler<IdentificationItem>();

        private static List<IdentificationItem> UpdateAllIdentificationItems(
            List<IdentificationItem> identificationItems)
        {
            var approvedIdentificationItems = identificationItems.DeepClone();

            foreach (var item in approvedIdentificationItems)
            {
                item.HasAccess = true;
                item.IsReidentified = true;
            }

            return approvedIdentificationItems;
        }
    }
}
