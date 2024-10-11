// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.UserAccesses
{
    public partial class UserAccessesControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<UserAccess> randomUserAccesses = CreateRandomUserAccesses();
            IQueryable<UserAccess> storageUserAccesses = randomUserAccesses.DeepClone();
            IQueryable<UserAccess> expectedUserAccess = storageUserAccesses.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAccess);

            var expectedActionResult =
                new ActionResult<IQueryable<UserAccess>>(expectedObjectResult);

            userAccessServiceMock
                .Setup(service => service.RetrieveAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccesses);

            // when
            ActionResult<IQueryable<UserAccess>> actualActionResult =
                await userAccessesController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAccessServiceMock
               .Verify(service => service.RetrieveAllUserAccessesAsync(),
                   Times.Once);

            userAccessServiceMock.VerifyNoOtherCalls();
        }
    }
}
