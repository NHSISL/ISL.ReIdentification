﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
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

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<object>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<object> actualActionResult =
                await this.reIdentificationController
                    .GetCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId),
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

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<object>(expectedBadRequestObjectResult);

            this.identificationCoordinationServiceMock.Setup(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<object> actualActionResult =
                await this.reIdentificationController
                    .GetCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.identificationCoordinationServiceMock.Verify(service =>
                service.ProcessCsvIdentificationRequestAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
