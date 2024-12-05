// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.Lookups
{
    public partial class LookupsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Lookup randomLookup = CreateRandomLookup();
            Lookup inputLookup = randomLookup;
            Lookup addedLookup = inputLookup.DeepClone();
            Lookup expectedLookup = addedLookup.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedLookup);

            var expectedActionResult =
                new ActionResult<Lookup>(expectedObjectResult);

            lookupServiceMock
                .Setup(service => service.AddLookupAsync(inputLookup))
                    .ReturnsAsync(addedLookup);

            // when
            ActionResult<Lookup> actualActionResult = await lookupsController.PostLookupAsync(randomLookup);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            lookupServiceMock
               .Verify(service => service.AddLookupAsync(inputLookup),
                   Times.Once);

            lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}