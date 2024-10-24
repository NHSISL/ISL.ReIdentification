// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.PdsDatas
{
    public partial class PdsDataControllerTests
    {
        [Fact]
        public async Task DeletePdsDataByIdAsyncShouldReturnPdsData()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            Guid inputId = randomPdsData.Id;
            PdsData storagePdsData = randomPdsData;
            PdsData expectedPdsData = storagePdsData.DeepClone();

            pdsDataServiceMock.Setup(service =>
                service.RemovePdsDataByIdAsync(inputId))
                .ReturnsAsync(storagePdsData);

            // when
            var result = await this.pdsDataController.DeletePdsDataByIdAsync(inputId);

            // then
            var actualResult = Assert.IsType<OkObjectResult>(result.Result);
            actualResult.StatusCode.Should().Be(200);
            actualResult.Value.Should().BeEquivalentTo(expectedPdsData);
        }
    }
}
