﻿// ---------------------------------------------------------
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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Lookup someLookup = CreateRandomLookup();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedBadRequestObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.PutLookupAsync(someLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Lookup someLookup = CreateRandomLookup();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedBadRequestObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.PutLookupAsync(someLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Lookup someLookup = CreateRandomLookup();
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
                service.ModifyLookupAsync(It.IsAny<Lookup>()))
                    .ThrowsAsync(lookupValidationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.PutLookupAsync(someLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsLookupErrorOccurredAsync()
        {
            // given
            Lookup someLookup = CreateRandomLookup();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsLookupException =
                new AlreadyExistsLookupException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var lookupDependencyValidationException =
                new LookupDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsLookupException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsLookupException);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedConflictObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()))
                    .ThrowsAsync(lookupDependencyValidationException);

            // when
            ActionResult<Lookup> actualActionResult =
                await this.lookupsController.PutLookupAsync(someLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.ModifyLookupAsync(It.IsAny<Lookup>()),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}