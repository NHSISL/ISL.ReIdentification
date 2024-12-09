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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.DeleteUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.DeleteUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFogreementException =
                new NotFoundUserAgreementException(
                    userAgreementId: someId);

            var userAgreementValidationException =
                new UserAgreementValidationException(
                    message: someMessage,
                    innerException: notFogreementException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFogreementException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedNotFoundObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(userAgreementValidationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.DeleteUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockegreementException =
                new LockedUserAgreementException(
                    message: someMessage,
                    innerException: someInnerException);

            var userAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: someMessage,
                    innerException: lockegreementException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockegreementException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedConflictObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(userAgreementDependencyValidationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.DeleteUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
