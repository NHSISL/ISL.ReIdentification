﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
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
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessProcessingServiceMock.Setup(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.GetUserAccessByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessProcessingServiceMock.Verify(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAccessProcessingServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessProcessingServiceMock.Setup(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.GetUserAccessByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessProcessingServiceMock.Verify(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundUserAccessException =
                new NotFoundUserAccessException(
                    message: someMessage);

            var userAccessProcessingDependencyValidationException =
                new UserAccessProcessingDependencyValidationException(
                    message: someMessage,
                    innerException: notFoundUserAccessException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundUserAccessException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedNotFoundObjectResult);

            this.userAccessProcessingServiceMock.Setup(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(userAccessProcessingDependencyValidationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.GetUserAccessByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessProcessingServiceMock.Verify(service =>
                service.RetrieveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
