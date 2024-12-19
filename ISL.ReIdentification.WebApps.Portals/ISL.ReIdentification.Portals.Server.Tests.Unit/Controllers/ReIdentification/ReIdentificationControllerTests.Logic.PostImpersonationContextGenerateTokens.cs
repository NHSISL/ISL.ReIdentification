// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPostImpersonationContextgenerateTokensWithTokensAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid impersonationContextId = randomId;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest updatedAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = updatedAccessRequest.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAccessRequest);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedObjectResult);

            identificationCoordinationServiceMock
                .Setup(service => service.ExpireRenewImpersonationContextTokensAsync(impersonationContextId))
                    .ReturnsAsync(updatedAccessRequest);

            // when
            ActionResult<AccessRequest> actualActionResult = await reIdentificationController
                .PostImpersonationContextGenerateTokensAsync(impersonationContextId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
               .Verify(service => service.ExpireRenewImpersonationContextTokensAsync(impersonationContextId),
                   Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
