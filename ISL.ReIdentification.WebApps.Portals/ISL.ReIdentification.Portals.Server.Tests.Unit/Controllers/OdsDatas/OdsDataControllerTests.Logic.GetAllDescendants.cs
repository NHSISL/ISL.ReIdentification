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

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAllDescendantsAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = randomOdsData;
            List<OdsData> childrenOdsDatas = CreateRandomOdsDataChildren(storageOdsData.OdsHierarchy);
            List<OdsData> grandchildrenOdsDatas = new List<OdsData>();

            foreach (var odsData in childrenOdsDatas)
            {
                List<OdsData> grandchildren = CreateRandomOdsDataChildren(odsData.OdsHierarchy);
                grandchildrenOdsDatas.AddRange(grandchildren);
            }

            List<OdsData> children = childrenOdsDatas;
            List<OdsData> allOdsDatas = new List<OdsData>();
            allOdsDatas.AddRange(children);
            allOdsDatas.AddRange(grandchildrenOdsDatas);
            List<OdsData> expectedOdsDatas = allOdsDatas.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsDatas);

            var expectedActionResult =
                new ActionResult<List<OdsData>>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.RetrieveAllDecendentsByParentId(inputOdsData.Id))
                    .ReturnsAsync(allOdsDatas);

            // when
            ActionResult<List<OdsData>> actualActionResult = await odsDataController
                .GetAllDescendants(inputOdsData.Id);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.RetrieveAllDecendentsByParentId(inputOdsData.Id),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
