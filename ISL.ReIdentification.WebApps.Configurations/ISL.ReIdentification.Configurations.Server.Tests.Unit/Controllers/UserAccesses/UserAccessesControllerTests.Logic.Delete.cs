﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            Guid inputId = randomUserAccess.Id;
            UserAccess storageUserAccess = randomUserAccess;
            UserAccess expectedUserAccess = storageUserAccess.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAccess);

            var expectedActionResult =
                new ActionResult<UserAccess>(expectedObjectResult);

            userAccessProcessingServiceMock
                .Setup(service => service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageUserAccess);

            // when
            ActionResult<UserAccess> actualActionResult =
                await userAccessesController.DeleteUserAccessByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAccessProcessingServiceMock
                .Verify(service => service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            userAccessProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
