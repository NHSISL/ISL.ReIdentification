// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<ImpersonationContext> randomImpersonationContexts = CreateRandomImpersonationContexts();
            IQueryable<ImpersonationContext> storageImpersonationContexts = randomImpersonationContexts.DeepClone();
            IQueryable<ImpersonationContext> expectedImpersonationContext = storageImpersonationContexts.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedImpersonationContext);

            var expectedActionResult =
                new ActionResult<IQueryable<ImpersonationContext>>(expectedObjectResult);

            impersonationContextServiceMock
                .Setup(service => service.RetrieveAllImpersonationContextsAsync())
                    .ReturnsAsync(storageImpersonationContexts);

            // when
            ActionResult<IQueryable<ImpersonationContext>> actualActionResult =
                await impersonationContextsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            impersonationContextServiceMock
               .Verify(service => service.RetrieveAllImpersonationContextsAsync(),
                   Times.Once);

            impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
