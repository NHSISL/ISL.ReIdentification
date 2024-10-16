// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            Guid inputId = randomImpersonationContext.Id;
            ImpersonationContext storageImpersonationContext = randomImpersonationContext;
            ImpersonationContext expectedImpersonationContext = storageImpersonationContext.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedImpersonationContext);

            var expectedActionResult =
                new ActionResult<ImpersonationContext>(expectedObjectResult);

            impersonationContextServiceMock
                .Setup(service => service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageImpersonationContext);

            // when
            ActionResult<ImpersonationContext> actualActionResult =
                await impersonationContextsController.GetImpersonationContextByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            impersonationContextServiceMock
                .Verify(service => service.RetrieveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
