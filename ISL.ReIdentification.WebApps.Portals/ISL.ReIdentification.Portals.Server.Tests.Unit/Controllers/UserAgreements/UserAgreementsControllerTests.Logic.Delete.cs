// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAgreements
{
    public partial class UserAgreementsControllerTests
    {
        [Fact]
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            Guid inputId = randomUserAgreement.Id;
            UserAgreement storageUserAgreement = randomUserAgreement;
            UserAgreement expectedUserAgreement = storageUserAgreement.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAgreement);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedObjectResult);

            userAgreementServiceMock
                .Setup(service => service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageUserAgreement);

            // when
            ActionResult<UserAgreement> actualActionResult =
                await userAgreementsController.DeleteUserAgreementByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAgreementServiceMock
                .Verify(service => service.RemoveUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
