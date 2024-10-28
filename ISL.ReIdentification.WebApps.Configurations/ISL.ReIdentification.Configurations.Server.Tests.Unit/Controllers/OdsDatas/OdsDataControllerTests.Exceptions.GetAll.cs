// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<OdsData> someOdsDatas = CreateRandomOdsDatas();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<OdsData>>(expectedInternalServerErrorObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.RetrieveAllOdsDatasAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<OdsData>> actualActionResult =
                await this.odsDataController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RetrieveAllOdsDatasAsync(),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
