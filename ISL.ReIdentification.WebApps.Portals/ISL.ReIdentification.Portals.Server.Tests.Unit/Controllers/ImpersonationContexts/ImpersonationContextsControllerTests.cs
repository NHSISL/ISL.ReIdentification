// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using ISL.ReIdentification.Configurations.Server.Controllers;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests : RESTFulController
    {
        private readonly Mock<IImpersonationContextService> impersonationContextServiceMock;
        private readonly ImpersonationContextsController impersonationContextsController;

        public ImpersonationContextsControllerTests()
        {
            impersonationContextServiceMock = new Mock<IImpersonationContextService>();
            impersonationContextsController = new ImpersonationContextsController(impersonationContextServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ImpersonationContextValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ImpersonationContextDependencyValidationException(
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
                new ImpersonationContextDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ImpersonationContextServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateImpersonationContextFiller().Create();

        private static IQueryable<ImpersonationContext> CreateRandomImpersonationContexts()
        {
            return CreateImpersonationContextFiller()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<ImpersonationContext> CreateImpersonationContextFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);

            return filler;
        }
    }
}
