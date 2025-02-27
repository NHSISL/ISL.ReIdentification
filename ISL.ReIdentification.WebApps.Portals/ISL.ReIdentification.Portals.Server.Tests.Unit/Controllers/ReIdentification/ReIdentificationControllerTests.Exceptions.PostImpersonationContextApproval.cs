// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostImpersonationContextApprovalIfValidationErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            ApprovalRequest someApprovalRequest = CreateRandomApprovalRequest();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult = expectedBadRequestObjectResult;

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ImpersonationContextApprovalAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.reIdentificationController.PostImpersonationContextApprovalAsync(someApprovalRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ImpersonationContextApprovalAsync(It.IsAny<Guid>(), It.IsAny<bool>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorPostImpersonationContextApprovaIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            ApprovalRequest someApprovalRequest = CreateRandomApprovalRequest();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult = expectedInternalServerErrorObjectResult;

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ImpersonationContextApprovalAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult actualActionResult =
                await this.reIdentificationController.PostImpersonationContextApprovalAsync(someApprovalRequest);

            // then
            actualActionResult.Should().BeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ImpersonationContextApprovalAsync(It.IsAny<Guid>(), It.IsAny<bool>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
