// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Unit.Controllers.Audits
{
    public partial class AuditsControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordOnGetByAuditTypeAsync()
        {
            // given
            Audit randomAudit = CreateRandomAudit();
            string inputAuditType = randomAudit.AuditType;
            Audit storageAudit = randomAudit;
            Audit expectedAudit = storageAudit.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedAudit);

            var expectedActionResult =
                new ActionResult<Audit>(expectedObjectResult);

            auditServiceMock
                .Setup(service => service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()))
                    .ReturnsAsync(storageAudit);

            // when
            ActionResult<Audit> actualActionResult = await auditsController.GetAuditByAuditTypeAsync(inputAuditType);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            auditServiceMock
                .Verify(service => service.RetrieveAuditByAuditTypeAsync(It.IsAny<string>()),
                    Times.Once);

            auditServiceMock.VerifyNoOtherCalls();
        }
    }
}
