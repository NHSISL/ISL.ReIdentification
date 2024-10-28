// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            UserAccess storageUserAccess = inputUserAccess.DeepClone();
            UserAccess expectedUserAccess = storageUserAccess.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAccess);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedObjectResult);

            userAccessProcessingService
                .Setup(service => service.ModifyUserAccessAsync(inputUserAccess))
                    .ReturnsAsync(storageUserAccess);

            // when
            ActionResult<UserAccess> actualActionResult = await userAccessesController
                .PutUserAccessAsync(randomUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAccessProcessingService
               .Verify(service => service.ModifyUserAccessAsync(inputUserAccess),
                   Times.Once);

            userAccessProcessingService.VerifyNoOtherCalls();
        }
    }
}
