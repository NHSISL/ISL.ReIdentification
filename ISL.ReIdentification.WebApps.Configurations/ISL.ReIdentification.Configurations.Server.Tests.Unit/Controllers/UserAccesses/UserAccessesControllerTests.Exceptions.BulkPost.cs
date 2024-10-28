// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
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
        public async Task ShouldReturnBadRequestOnBulkPostIfValidationErrorOccurredAsync(Xeption validationException)
        {
            // given
            BulkUserAccess someBulkUserAccess = CreateRandomBulkUserAccess();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessProcessingServiceMock.Setup(service =>
                service.BulkAddRemoveUserAccessAsync(It.IsAny<BulkUserAccess>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult actualActionResult =
                await this.userAccessesController.PostBulkUserAccessAsync(someBulkUserAccess);

            // then

            this.userAccessProcessingServiceMock.Verify(service =>
                service.BulkAddRemoveUserAccessAsync(It.IsAny<BulkUserAccess>()),
                    Times.Once);

            this.userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnBulkPostIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            BulkUserAccess someBulkUserAccess = CreateRandomBulkUserAccess();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedBadRequestObjectResult);

            this.userAccessProcessingServiceMock.Setup(service =>
                service.BulkAddRemoveUserAccessAsync(It.IsAny<BulkUserAccess>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<UserAccess> actualActionResult =
                await this.userAccessesController.PostBulkUserAccessAsync(someBulkUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessProcessingServiceMock.Verify(service =>
                service.BulkAddRemoveUserAccessAsync(It.IsAny<BulkUserAccess>()),
                    Times.Once);

            this.userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
