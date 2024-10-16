// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = randomUserAccess;
            UserAccess addedUserAccess = inputUserAccess.DeepClone();
            UserAccess expectedUserAccess = addedUserAccess.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedUserAccess);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedObjectResult);

            userAccessServiceMock
                .Setup(service => service.AddUserAccessAsync(inputUserAccess))
                    .ReturnsAsync(addedUserAccess);

            // when
            ActionResult<UserAccess> actualActionResult = await userAccessesController
                .PostUserAccessAsync(randomUserAccess);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAccessServiceMock
               .Verify(service => service.AddUserAccessAsync(inputUserAccess),
                   Times.Once);

            userAccessServiceMock.VerifyNoOtherCalls();
        }
    }
}
