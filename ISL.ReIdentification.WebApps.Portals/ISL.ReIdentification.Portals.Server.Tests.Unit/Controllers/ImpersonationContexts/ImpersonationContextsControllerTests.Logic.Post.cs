// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext addedImpersonationContext = inputImpersonationContext.DeepClone();
            ImpersonationContext expectedImpersonationContext = addedImpersonationContext.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedImpersonationContext);

            var expectedActionResult =
                new ActionResult<ImpersonationContext>(expectedObjectResult);

            impersonationContextServiceMock
                .Setup(service => service.AddImpersonationContextAsync(inputImpersonationContext))
                    .ReturnsAsync(addedImpersonationContext);

            // when
            ActionResult<ImpersonationContext> actualActionResult = await impersonationContextsController
                .PostImpersonationContextAsync(randomImpersonationContext);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            impersonationContextServiceMock
               .Verify(service => service.AddImpersonationContextAsync(inputImpersonationContext),
                   Times.Once);

            impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
