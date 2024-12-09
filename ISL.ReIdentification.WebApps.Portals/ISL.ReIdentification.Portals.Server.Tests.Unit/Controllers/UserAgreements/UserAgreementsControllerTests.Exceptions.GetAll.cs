// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.UserAgreements
{
    public partial class UserAgreementsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<UserAgreement> someUserAgreements = CreateRandomUserAgreements();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<UserAgreement>>(expectedInternalServerErrorObjectResult);

            this.userAgreementServiceMock.Setup(service =>
                service.RetrieveAllUserAgreementsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<UserAgreement>> actualActionResult =
                await this.userAgreementsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userAgreementServiceMock.Verify(service =>
                service.RetrieveAllUserAgreementsAsync(),
                    Times.Once);

            this.userAgreementServiceMock.VerifyNoOtherCalls();
        }
    }
}
