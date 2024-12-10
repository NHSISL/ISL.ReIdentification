// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement storageUserAgreement = inputUserAgreement.DeepClone();
            UserAgreement expectedUserAgreement = storageUserAgreement.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAgreement);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedObjectResult);

            userAgreementServiceMock
                .Setup(service => service.ModifyUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(storageUserAgreement);

            // when
            ActionResult<UserAgreement> actualActionResult = await userAgreementsController
                .PutUserAgreementAsync(randomUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAgreementServiceMock
               .Verify(service => service.ModifyUserAgreementAsync(inputUserAgreement),
                   Times.Once);

            userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
