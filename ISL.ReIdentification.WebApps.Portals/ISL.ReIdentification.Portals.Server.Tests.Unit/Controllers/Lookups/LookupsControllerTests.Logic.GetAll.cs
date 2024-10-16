// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
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
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            IQueryable<Lookup> randomLookups = CreateRandomLookups();
            IQueryable<Lookup> storageLookups = randomLookups.DeepClone();
            IQueryable<Lookup> expectedLookup = storageLookups.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedLookup);

            var expectedActionResult =
                new ActionResult<IQueryable<Lookup>>(expectedObjectResult);

            lookupServiceMock
                .Setup(service => service.RetrieveAllLookupsAsync())
                    .ReturnsAsync(storageLookups);

            // when
            ActionResult<IQueryable<Lookup>> actualActionResult = await lookupsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            lookupServiceMock
               .Verify(service => service.RetrieveAllLookupsAsync(),
                   Times.Once);

            lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}
