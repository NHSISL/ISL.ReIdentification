// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
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
        public async Task ShouldReturnBadRequestOnGetIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<object>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<object> actualActionResult =
                await this.reIdentificationController
                    .GetCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId, someReason);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            string someReason = GetRandomString();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<object>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<object> actualActionResult =
                await this.reIdentificationController
                    .GetCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId, someReason);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedWhenUserDoesNotHavePermissionsAsync()
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
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
                    innerException: unauthorizedIdentificationCoordinationException);

            UnauthorizedObjectResult expectedUnauthorizedObjectResult =
                Unauthorized(identificationCoordinationDependencyValidationException.InnerException);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason))
                    .ThrowsAsync(identificationCoordinationDependencyValidationException);

            // when
            ActionResult<object> actualActionResult =
                await this.reIdentificationController
                    .GetCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId, someReason);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedUnauthorizedObjectResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId, someReason),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
