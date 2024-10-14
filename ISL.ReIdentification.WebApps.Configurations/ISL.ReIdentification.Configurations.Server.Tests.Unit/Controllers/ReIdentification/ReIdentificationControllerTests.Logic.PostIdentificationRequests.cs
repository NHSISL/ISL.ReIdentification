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

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest;
            AccessRequest updatedAccessRequest = inputAccessRequest.DeepClone();

            updatedAccessRequest.IdentificationRequest.IdentificationItems =
                UpdateAllIdentificationItems(updatedAccessRequest.IdentificationRequest.IdentificationItems);

            AccessRequest addedAccessRequest = updatedAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = addedAccessRequest.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedAccessRequest);

            var expectedActionResult =
                new ActionResult<AccessRequest>(expectedObjectResult);

            identificationCoordinationServiceMock
                .Setup(service => service.ProcessIdentificationRequestsAsync(inputAccessRequest))
                    .ReturnsAsync(addedAccessRequest);

            // when
            ActionResult<AccessRequest> actualActionResult = await reIdentificationController
                .PostIdentificationRequestsAsync(randomAccessRequest);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            identificationCoordinationServiceMock
               .Verify(service => service.ProcessIdentificationRequestsAsync(inputAccessRequest),
                   Times.Once);

            identificationCoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
