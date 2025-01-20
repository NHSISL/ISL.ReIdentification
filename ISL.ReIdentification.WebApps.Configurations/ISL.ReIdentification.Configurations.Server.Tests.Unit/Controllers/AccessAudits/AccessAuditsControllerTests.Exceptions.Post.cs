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

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PostAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            AccessAudit someAccessAudit = CreateRandomAccessAudit();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedBadRequestObjectResult);

            this.accessAuditServiceMock.Setup(service =>
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PostAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsAccessAuditErrorOccurredAsync()
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
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()))
                    .ThrowsAsync(accessAuditDependencyValidationException);

            // when
            ActionResult<AccessAudit> actualActionResult =
                await this.accessAuditsController.PostAccessAuditAsync(someAccessAudit);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.accessAuditServiceMock.Verify(service =>
                service.AddAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}