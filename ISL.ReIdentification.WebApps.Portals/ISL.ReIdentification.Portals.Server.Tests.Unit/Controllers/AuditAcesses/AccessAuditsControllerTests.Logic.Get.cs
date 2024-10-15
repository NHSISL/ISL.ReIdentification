// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.AccessAudits
{
    public partial class AccessAuditsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            Guid inputId = randomAccessAudit.Id;
            AccessAudit storageAccessAudit = randomAccessAudit;
            AccessAudit expectedAccessAudit = storageAccessAudit.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAccessAudit);

            var expectedActionResult =
                new ActionResult<AccessAudit>(expectedObjectResult);

            accessAuditServiceMock
                .Setup(service => service.RetrieveAccessAuditByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageAccessAudit);

            // when
            ActionResult<AccessAudit> actualActionResult = await accessAuditsController.GetAccessAuditByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            accessAuditServiceMock
                .Verify(service => service.RetrieveAccessAuditByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            accessAuditServiceMock.VerifyNoOtherCalls();
        }
    }
}
