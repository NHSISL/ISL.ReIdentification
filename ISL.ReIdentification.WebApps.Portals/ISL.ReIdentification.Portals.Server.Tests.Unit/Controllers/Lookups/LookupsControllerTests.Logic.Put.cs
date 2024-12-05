// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            Lookup inputLookup = randomLookup;
            Lookup storageLookup = inputLookup.DeepClone();
            Lookup expectedLookup = storageLookup.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedLookup);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedObjectResult);

            lookupServiceMock
                .Setup(service => service.ModifyLookupAsync(inputLookup))
                    .ReturnsAsync(storageLookup);

            // when
            ActionResult<Lookup> actualActionResult = await lookupsController.PutLookupAsync(randomLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            lookupServiceMock
               .Verify(service => service.ModifyLookupAsync(inputLookup),
                   Times.Once);

            lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}
