// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.Lookups
{
    public partial class LookupsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            Guid inputId = randomLookup.Id;
            Lookup storageLookup = randomLookup;
            Lookup expectedLookup = storageLookup.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedLookup);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedObjectResult);

            lookupServiceMock
                .Setup(service => service.RetrieveLookupByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageLookup);

            // when
            ActionResult<Lookup> actualActionResult = await lookupsController.GetLookupByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            lookupServiceMock
                .Verify(service => service.RetrieveLookupByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}
