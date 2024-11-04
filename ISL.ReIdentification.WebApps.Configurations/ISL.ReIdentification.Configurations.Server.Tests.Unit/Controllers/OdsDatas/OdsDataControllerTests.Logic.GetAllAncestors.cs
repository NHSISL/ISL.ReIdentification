// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldReturnRecordsOnGetAllAncestorsAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData storageOdsData = randomOdsData;
            List<OdsData> childrenOdsDatas = CreateRandomOdsDataChildren(storageOdsData.OdsHierarchy, 1);
            List<OdsData> grandChildrenOdsDatas = CreateRandomOdsDataChildren(childrenOdsDatas[0].OdsHierarchy, 1);
            List<OdsData> storageOdsDatas = new List<OdsData> { storageOdsData };
            storageOdsDatas.AddRange(childrenOdsDatas);
            storageOdsDatas.AddRange(grandChildrenOdsDatas);
            List<OdsData> ancestors = new List<OdsData>();
            ancestors.AddRange(childrenOdsDatas);
            ancestors.Add(storageOdsData);
            Guid odsDataChildId = grandChildrenOdsDatas[0].Id;
            List<OdsData> expectedOdsDatas = ancestors.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsDatas);

            var expectedActionResult =
                new ActionResult<List<OdsData>>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.RetrieveAllAncestorsByChildId(odsDataChildId))
                    .ReturnsAsync(ancestors);

            // when
            ActionResult<List<OdsData>> actualActionResult = await odsDataController
                .GetAllAncestors(odsDataChildId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
               .Verify(service => service.RetrieveAllAncestorsByChildId(odsDataChildId),
                   Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
