// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostImpersonationContextGenerateTokensIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ExpireRenewImpersonationContextTokensAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostImpersonationContextGenerateTokensAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ExpireRenewImpersonationContextTokensAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostImpersonationContextGenerateTokensIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ExpireRenewImpersonationContextTokensAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostImpersonationContextGenerateTokensAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ExpireRenewImpersonationContextTokensAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
