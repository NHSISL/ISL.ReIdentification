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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.DeleteCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedBadRequestObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.DeleteCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
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
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(csvIdentificationRequestValidationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.DeleteCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedCsvIdentificationRequestException =
                new LockedCsvIdentificationRequestException(
                    message: someMessage,
                    innerException: someInnerException);

            var csvIdentificationRequestDependencyValidationException =
                new CsvIdentificationRequestDependencyValidationException(
                    message: someMessage,
                    innerException: lockedCsvIdentificationRequestException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedCsvIdentificationRequestException);

            var expectedActionResult =
                new ActionResult<CsvIdentificationRequest>(expectedConflictObjectResult);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(csvIdentificationRequestDependencyValidationException);

            // when
            ActionResult<CsvIdentificationRequest> actualActionResult =
                await this.csvIdentificationRequestsController.DeleteCsvIdentificationRequestByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RemoveCsvIdentificationRequestByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
        }
    }
}
