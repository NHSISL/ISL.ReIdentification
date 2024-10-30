// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.OdsDatas
{
    public partial class OdsDataControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetAllAncestorsIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Guid someGuid = Guid.NewGuid();
            IQueryable<OdsData> someOdsDatas = CreateRandomOdsDatas();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<List<OdsData>>(expectedInternalServerErrorObjectResult);

            this.odsDataServiceMock.Setup(service =>
                service.RetrieveAllAncestorsByChildId(someGuid))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<List<OdsData>> actualActionResult =
                await this.odsDataController.GetAllAncestors(someGuid);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.odsDataServiceMock.Verify(service =>
                service.RetrieveAllAncestorsByChildId(someGuid),
                    Times.Once);

            this.odsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
