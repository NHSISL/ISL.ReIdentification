// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            CsvIdentificationRequest someCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.PutCsvIdentificationRequestAsync(someCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            CsvIdentificationRequest someCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.PutCsvIdentificationRequestAsync(someCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            CsvIdentificationRequest someCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            string someMessage = GetRandomString();

            var notFoundCsvIdentificationRequestException =
                new NotFoundCsvIdentificationRequestException(
                    message: someMessage);

            var csvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: someMessage,
                    innerException: notFoundCsvIdentificationRequestException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundCsvIdentificationRequestException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedNotFoundObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()))
                    .ThrowsAsync(csvIdentificationRequestValidationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.PutCsvIdentificationRequestAsync(someCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsCsvIdentificationRequestErrorOccurredAsync()
        {
            // given
            CsvIdentificationRequest someCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsCsvIdentificationRequestException =
                new AlreadyExistsCsvIdentificationRequestException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var csvIdentificationRequestDependencyValidationException =
                new CsvIdentificationRequestDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsCsvIdentificationRequestException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsCsvIdentificationRequestException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedConflictObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()))
                    .ThrowsAsync(csvIdentificationRequestDependencyValidationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.PutCsvIdentificationRequestAsync(someCsvIdentificationRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.ModifyCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
