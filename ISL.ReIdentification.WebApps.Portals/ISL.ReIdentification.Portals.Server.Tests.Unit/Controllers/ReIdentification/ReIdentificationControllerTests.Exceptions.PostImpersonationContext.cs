﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldReturnBadRequestOnPostImpersonationContextIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.PersistsImpersonationContextAsync(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostImpersonationContextRequestAsync(someAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.PersistsImpersonationContextAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostImpersonationContextIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.PersistsImpersonationContextAsync(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostImpersonationContextRequestAsync(someAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.PersistsImpersonationContextAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
