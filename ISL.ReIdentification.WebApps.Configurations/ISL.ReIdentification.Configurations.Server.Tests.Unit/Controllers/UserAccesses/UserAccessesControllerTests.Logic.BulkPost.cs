// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnBulkPostAsync()
        {
            // given
            BulkUserAccess randomBulkUserAccess = CreateRandomBulkUserAccess();
            BulkUserAccess inputBulkUserAccess = randomBulkUserAccess;

            var expectedObjectResult = new CreatedObjectResult(null);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedObjectResult);

            // when
            ActionResult actualActionResult = await userAccessesController
                .PostBulkUserAccessAsync(inputBulkUserAccess);

            // then
            userAccessProcessingServiceMock
               .Verify(service => service.BulkAddRemoveUserAccessAsync(inputBulkUserAccess),
                   Times.Once);

            userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
