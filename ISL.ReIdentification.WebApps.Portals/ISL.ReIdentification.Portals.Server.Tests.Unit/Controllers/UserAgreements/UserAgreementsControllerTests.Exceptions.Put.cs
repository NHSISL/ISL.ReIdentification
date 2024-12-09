// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAgreements
{
    public partial class UserAgreementsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.PutUserAgreementAsync(someUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.PutUserAgreementAsync(someUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            string someMessage = GetRandomString();

            var notFoundUserAgreementException =
                new NotFoundUserAgreementException(
                    userAgreementId: someUserAgreement.Id);

            var userAgreementValidationException =
                new UserAgreementValidationException(
                    message: someMessage,
                    innerException: notFoundUserAgreementException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundUserAgreementException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedNotFoundObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()))
                    .ThrowsAsync(userAgreementValidationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.PutUserAgreementAsync(someUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsUserAgreementErrorOccurredAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsUserAgreementException =
                new AlreadyExistsUserAgreementException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var userAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsUserAgreementException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsUserAgreementException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedConflictObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()))
                    .ThrowsAsync(userAgreementDependencyValidationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.PutUserAgreementAsync(someUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.ModifyUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
