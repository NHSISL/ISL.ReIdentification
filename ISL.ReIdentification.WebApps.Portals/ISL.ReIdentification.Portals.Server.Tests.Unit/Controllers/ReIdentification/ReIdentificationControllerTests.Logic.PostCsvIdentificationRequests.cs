// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostCsvAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest addedAccessRequest = inputAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = addedAccessRequest.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedAccessRequest);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedObjectResult);

            identificationCoordinationServiceMock
                .Setup(service => service.PersistsCsvIdentificationRequestAsync(inputAccessRequest))
                    .ReturnsAsync(addedAccessRequest);

            // when
            ActionResult<AccessRequest> actualActionResult = await reIdentificationController
                .PostCsvIdentificationRequestAsync(randomAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
               .Verify(service => service.PersistsCsvIdentificationRequestAsync(inputAccessRequest),
                   Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}