﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldRemoveRecordOnDeleteByIdsAsync()
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
                .Setup(service => service.RemoveOdsDataByIdAsync(inputId))
                    .ReturnsAsync(storageOdsData);

            // when
            ActionResult<OdsData> actualActionResult = await odsDataController.DeleteOdsDataByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            odsDataServiceMock
                .Verify(service => service.RemoveOdsDataByIdAsync(inputId),
                    Times.Once);

            odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
