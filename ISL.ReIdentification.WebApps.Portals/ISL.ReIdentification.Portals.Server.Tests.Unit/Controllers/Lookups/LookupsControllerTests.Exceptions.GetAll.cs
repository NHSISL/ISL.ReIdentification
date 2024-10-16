// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.Lookups
{
    public partial class LookupsControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            IQueryable<Lookup> someLookups = CreateRandomLookups();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Lookup>>(expectedInternalServerErrorObjectResult);

            this.lookupServiceMock.Setup(service =>
                service.RetrieveAllLookupsAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Lookup>> actualActionResult =
                await this.lookupsController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.lookupServiceMock.Verify(service =>
                service.RetrieveAllLookupsAsync(),
                    Times.Once);

            this.lookupServiceMock.VerifyNoOtherCalls();
        }
    }
}
