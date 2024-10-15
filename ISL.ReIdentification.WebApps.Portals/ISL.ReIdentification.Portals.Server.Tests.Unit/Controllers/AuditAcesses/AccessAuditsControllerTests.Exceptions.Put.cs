// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PutAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PutAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();
            string someMessage = GetRandomString();

            var notFoundAccessAuditException =
                new NotFoundAccessAuditException(
                    message: someMessage);

            var accessAuditValidationException =
                new AccessAuditValidationException(
                    message: someMessage,
                    innerException: notFoundAccessAuditException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundAccessAuditException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedNotFoundObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(accessAuditValidationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PutAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsAccessAuditErrorOccurredAsync()
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsAccessAuditException =
                new AlreadyExistsAccessAuditException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var accessAuditDependencyValidationException =
                new AccessAuditDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsAccessAuditException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsAccessAuditException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedConflictObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(accessAuditDependencyValidationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PutAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.ModifyAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
