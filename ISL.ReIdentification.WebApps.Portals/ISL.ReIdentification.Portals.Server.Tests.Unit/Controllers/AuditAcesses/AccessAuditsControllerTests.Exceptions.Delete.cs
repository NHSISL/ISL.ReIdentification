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
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.DeleteAccessAuditByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.DeleteAccessAuditByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(accessAuditValidationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.DeleteAccessAuditByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedAccessAuditException =
                new LockedAccessAuditException(
                    message: someMessage,
                    innerException: someInnerException);

            var accessAuditDependencyValidationException =
                new AccessAuditDependencyValidationException(
                    message: someMessage,
                    innerException: lockedAccessAuditException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedAccessAuditException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedConflictObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(accessAuditDependencyValidationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.DeleteAccessAuditByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.RemoveAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
