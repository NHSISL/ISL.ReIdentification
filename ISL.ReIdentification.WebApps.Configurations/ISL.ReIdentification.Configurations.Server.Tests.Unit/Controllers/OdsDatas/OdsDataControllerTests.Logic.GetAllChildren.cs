// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAllChildrenAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = randomOdsData;
            List<OdsData> randomOdsDatas = CreateRandomOdsDataChildren(storageOdsData.OdsHierarchy);
            List<OdsData> children = randomOdsDatas;
            List<OdsData> expectedOdsDatas = children.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsDatas);

            var expectedActionResult =
                new ActionResult<List<OdsData>>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.RetrieveChildrenByParentId(inputOdsData.Id))
                    .ReturnsAsync(children);

            // when
            ActionResult<List<OdsData>> actualActionResult = await odsDataController
                .GetAllChildren(inputOdsData.Id);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.RetrieveChildrenByParentId(inputOdsData.Id),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
