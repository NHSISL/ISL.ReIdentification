// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<UserAccess> someUserAccesses = CreateRandomUserAccesses();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<UserAccess>>(expectedInternalServerErrorObjectResult);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<UserAccess>> actualActionResult =
                await this.userAccessesController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
        }
    }
}
