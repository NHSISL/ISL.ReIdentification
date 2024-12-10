// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAgreements
{
    public partial class UserAgreementsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement inputUserAgreement = randomUserAgreement;
            UserAgreement addedUserAgreement = inputUserAgreement.DeepClone();
            UserAgreement expectedUserAgreement = addedUserAgreement.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedUserAgreement);

            var expectedActionResult =
                new ActionResult<UserAgreement>(expectedObjectResult);

            userAgreementServiceMock
                .Setup(service => service.AddUserAgreementAsync(inputUserAgreement))
                    .ReturnsAsync(addedUserAgreement);

            // when
            ActionResult<UserAgreement> actualActionResult = await userAgreementsController
                .PostUserAgreementAsync(randomUserAgreement);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAgreementServiceMock
               .Verify(service => service.AddUserAgreementAsync(inputUserAgreement),
                   Times.Once);

            userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
