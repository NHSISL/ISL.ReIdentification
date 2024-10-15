// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
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
                new ActionResult<ImpersonationContext>(expectedBadRequestObjectResult);

            this.impersonationContextServiceMock.Setup(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ImpersonationContext> actualActionResult =
                await this.impersonationContextsController.DeleteImpersonationContextByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.impersonationContextServiceMock.Verify(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<ImpersonationContext>(expectedBadRequestObjectResult);

            this.impersonationContextServiceMock.Setup(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<ImpersonationContext> actualActionResult =
                await this.impersonationContextsController.DeleteImpersonationContextByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.impersonationContextServiceMock.Verify(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundImpersonationContextException =
                new NotFoundImpersonationContextException(
                    message: someMessage);

            var impersonationContextValidationException =
                new ImpersonationContextValidationException(
                    message: someMessage,
                    innerException: notFoundImpersonationContextException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundImpersonationContextException);

            var expectedActionResult =
                new ActionResult<ImpersonationContext>(expectedNotFoundObjectResult);

            this.impersonationContextServiceMock.Setup(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(impersonationContextValidationException);

            // when
            ActionResult<ImpersonationContext> actualActionResult =
                await this.impersonationContextsController.DeleteImpersonationContextByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.impersonationContextServiceMock.Verify(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnLockedOnDeleteIfRecordIsLockedAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var lockedImpersonationContextException =
                new LockedImpersonationContextException(
                    message: someMessage,
                    innerException: someInnerException);

            var impersonationContextDependencyValidationException =
                new ImpersonationContextDependencyValidationException(
                    message: someMessage,
                    innerException: lockedImpersonationContextException);

            LockedObjectResult expectedConflictObjectResult =
                Locked(lockedImpersonationContextException);

            var expectedActionResult =
                new ActionResult<ImpersonationContext>(expectedConflictObjectResult);

            this.impersonationContextServiceMock.Setup(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(impersonationContextDependencyValidationException);

            // when
            ActionResult<ImpersonationContext> actualActionResult =
                await this.impersonationContextsController.DeleteImpersonationContextByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.impersonationContextServiceMock.Verify(service =>
                service.RemoveImpersonationContextByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.impersonationContextServiceMock.VerifyNoOtherCalls();
        }
    }
}
