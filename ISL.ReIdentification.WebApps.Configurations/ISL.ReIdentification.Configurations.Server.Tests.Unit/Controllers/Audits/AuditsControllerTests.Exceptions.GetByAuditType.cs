// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.Audits
{
    public partial class AuditsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnGetByAuditTypeIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            string someString = GetRandomString();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Audit>(expectedBadRequestObjectResult);

            this.auditServiceMock.Setup(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Audit> actualActionResult =
                await this.auditsController.GetAuditByAuditTypeAsync(someString);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.auditServiceMock.Verify(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()),
                    Times.Once);

            this.auditServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByAuditTypeIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            string someString = GetRandomString();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Audit>(expectedBadRequestObjectResult);

            this.auditServiceMock.Setup(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Audit> actualActionResult =
                await this.auditsController.GetAuditByAuditTypeAsync(someString);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.auditServiceMock.Verify(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()),
                    Times.Once);

            this.auditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByAuditTypeIfItemDoesNotExistAsync()
        {
            // given
            string someString = GetRandomString();
            string someMessage = GetRandomString();

            var notFoundAuditException =
                new NotFoundAuditException(
                    message: someMessage);

            var auditValidationException =
                new AuditValidationException(
                    message: someMessage,
                    innerException: notFoundAuditException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundAuditException);

            var expectedActionResult =
                new ActionResult<Audit>(expectedNotFoundObjectResult);

            this.auditServiceMock.Setup(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()))
                    .ThrowsAsync(auditValidationException);

            // when
            ActionResult<Audit> actualActionResult =
                await this.auditsController.GetAuditByAuditTypeAsync(someString);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.auditServiceMock.Verify(service =>
                service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()),
                    Times.Once);

            this.auditServiceMock.VerifyNoOtherCalls();
        }
    }
}
