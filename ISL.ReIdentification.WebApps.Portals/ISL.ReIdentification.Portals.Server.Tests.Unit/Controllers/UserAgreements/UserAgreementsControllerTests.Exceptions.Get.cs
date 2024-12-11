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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.GetUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<UserAgreement>(expectedBadRequestObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.GetUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundUserAgreementException =
                new NotFoundUserAgreementException(
                    userAgreementId: someId);

            var userAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: someMessage,
                    innerException: notFoundUserAgreementException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundUserAgreementException);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedNotFoundObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(userAgreementDependencyValidationException);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await this.userAgreementsController.GetUserAgreementByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RetrieveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
