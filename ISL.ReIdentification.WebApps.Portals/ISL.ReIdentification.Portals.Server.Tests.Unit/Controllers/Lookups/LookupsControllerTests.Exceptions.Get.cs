// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.Lookups
{
    public partial class LookupsControllerTests
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
                new ActionResult<Lookup>(expectedBadRequestObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.GetLookupByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<Lookup>(expectedBadRequestObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.GetLookupByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundLookupException =
                new NotFoundLookupException(
                    message: someMessage);

            var lookupValidationException =
                new LookupValidationException(
                    message: someMessage,
                    innerException: notFoundLookupException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundLookupException);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedNotFoundObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(lookupValidationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.GetLookupByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.RetrieveLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}
