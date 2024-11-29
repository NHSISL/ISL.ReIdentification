// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            Guid inputId = randomOdsData.Id;
            OdsData storageOdsData = randomOdsData;
            OdsData expectedOdsData = storageOdsData.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedOdsData);

            var expectedActionResult =
                new ActionResult<OdsData>(expectedObjectResult);

            odsDataServiceMock
                .Setup(service => service.RetrieveOdsDataByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageOdsData);

            // when
            ActionResult<OdsData> actualActionResult = await odsDataController.GetOdsDataByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
                .Verify(service => service.RetrieveOdsDataByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
