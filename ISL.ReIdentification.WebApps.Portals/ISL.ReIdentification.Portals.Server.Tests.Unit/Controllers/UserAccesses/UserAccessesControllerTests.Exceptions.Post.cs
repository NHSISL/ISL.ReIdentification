// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.PostUserAccessAsync(someUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.PostUserAccessAsync(someUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsUserAccessErrorOccurredAsync()
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsUserAccessException =
                new AlreadyExistsUserAccessException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var userAccessDependencyValidationException =
                new UserAccessDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsUserAccessException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsUserAccessException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedConflictObjectResult);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()))
                    .ThrowsAsync(userAccessDependencyValidationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.PostUserAccessAsync(someUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
        }
    }
}
