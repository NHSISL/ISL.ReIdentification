// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<OdsData> randomOdsDatas = CreateRandomOdsDatas();
            IQueryable<OdsData> storageOdsDatas = randomOdsDatas.DeepClone();
            IQueryable<OdsData> expectedOdsData = storageOdsDatas.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsData);

            var expectedActionResult =
                new ActionResult<IQueryable<OdsData>>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.RetrieveAllOdsDatasAsync())
                    .ReturnsAsync(storageOdsDatas);

            // when
            ActionResult<IQueryable<OdsData>> actualActionResult = await odsDataController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.RetrieveAllOdsDatasAsync(),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
