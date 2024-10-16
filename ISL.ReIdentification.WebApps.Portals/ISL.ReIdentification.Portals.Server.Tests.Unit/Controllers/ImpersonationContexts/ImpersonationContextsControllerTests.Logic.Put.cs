// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext inputImpersonationContext = randomImpersonationContext;
            ImpersonationContext storageImpersonationContext = inputImpersonationContext.DeepClone();
            ImpersonationContext expectedImpersonationContext = storageImpersonationContext.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedImpersonationContext);

            var expectedActionResult =
                new ActionResult<ImpersonationContext>(expectedObjectResult);

            impersonationContextServiceMock
                .Setup(service => service.ModifyImpersonationContextAsync(inputImpersonationContext))
                    .ReturnsAsync(storageImpersonationContext);

            // when
            ActionResult<ImpersonationContext> actualActionResult = await impersonationContextsController
                .PutImpersonationContextAsync(randomImpersonationContext);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            impersonationContextServiceMock
               .Verify(service => service.ModifyImpersonationContextAsync(inputImpersonationContext),
                   Times.Once);

            impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
