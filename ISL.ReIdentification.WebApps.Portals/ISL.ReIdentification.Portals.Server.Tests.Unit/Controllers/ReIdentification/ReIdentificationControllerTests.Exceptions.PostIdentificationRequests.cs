// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
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
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostIdentificationRequestsAsync(someAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostIdentificationRequestsAsync(someAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedOnPostWhenUserDoesNotHavePermissionsAsync()
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            string someReason = GetRandomString();
            string someMessage = GetRandomString();

            var unauthorizedAccessOrchestrationException =
                new UnauthorizedAccessOrchestrationException(
                    message: someMessage);

            var unauthorizedIdentificationCoordinationException =
                new UnauthorizedIdentificationCoordinationException(
                    message: someMessage,
                    innerException: unauthorizedAccessOrchestrationException);

            var identificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: someMessage,
                    innerException: unauthorizedAccessOrchestrationException);

            UnauthorizedObjectResult expectedUnauthorizedObjectResult =
                Unauthorized(identificationCoordinationDependencyValidationException.InnerException);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()))
                    .ThrowsAsync(identificationCoordinationDependencyValidationException);

            // when
            ActionResult<AccessRequest> actualActionResult =
                await this.reIdentificationController.PostIdentificationRequestsAsync(someAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedUnauthorizedObjectResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessIdentificationRequestsAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
