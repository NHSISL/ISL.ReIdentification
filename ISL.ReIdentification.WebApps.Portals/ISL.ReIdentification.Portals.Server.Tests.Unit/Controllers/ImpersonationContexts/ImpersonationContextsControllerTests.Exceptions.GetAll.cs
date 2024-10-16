// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<ImpersonationContext> someImpersonationContexts = CreateRandomImpersonationContexts();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<ImpersonationContext>>(expectedInternalServerErrorObjectResult);

            this.impersonationContextServiceMock.Setup(service =>
                service.RetrieveAllImpersonationContextsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<ImpersonationContext>> actualActionResult =
                await this.impersonationContextsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.impersonationContextServiceMock.Verify(service =>
                service.RetrieveAllImpersonationContextsAsync(),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
