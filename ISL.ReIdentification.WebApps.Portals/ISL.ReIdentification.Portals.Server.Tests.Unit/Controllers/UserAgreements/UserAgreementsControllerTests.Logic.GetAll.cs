// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<UserAgreement> randomUserAgreements = CreateRandomUserAgreements();
            IQueryable<UserAgreement> storageUserAgreements = randomUserAgreements.DeepClone();
            IQueryable<UserAgreement> expectedUserAgreement = storageUserAgreements.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUserAgreement);

            var expectedActionResult =
                new ActionResult<IQueryable<UserAgreement>>(expectedObjectResult);

            userAgreementServiceMock
                .Setup(service => service.RetrieveAllUserAgreementsAsync())
                    .ReturnsAsync(storageUserAgreements);

            // when
            ActionResult<IQueryable<UserAgreement>> actualActionResult =
                await userAgreementsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userAgreementServiceMock
               .Verify(service => service.RetrieveAllUserAgreementsAsync(),
                   Times.Once);

            userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
