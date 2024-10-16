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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.GetCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.GetCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(csvIdentificationRequestValidationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.GetCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
