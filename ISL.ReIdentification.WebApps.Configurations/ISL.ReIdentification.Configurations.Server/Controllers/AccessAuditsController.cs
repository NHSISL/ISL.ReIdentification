// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessAuditsController : RESTFulController
    {
        private readonly IAccessAuditService accessAuditService;

        public AccessAuditsController(IAccessAuditService accessAuditService) =>
            this.accessAuditService = accessAuditService;

        [HttpPost]
        public async ValueTask<ActionResult<AccessAudit>> PostAccessAuditAsync(AccessAudit accessAudit)
        {
            try
            {
                AccessAudit addedAccessAudit =
                    await this.accessAuditService.AddAccessAuditAsync(accessAudit);

                return Created(addedAccessAudit);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
            {
                return BadRequest(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
               when (accessAuditDependencyValidationException.InnerException is AlreadyExistsAccessAuditException)
            {
                return Conflict(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
            {
                return BadRequest(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                return InternalServerError(accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                return InternalServerError(accessAuditServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<AccessAudit>>> GetAsync()
        {
            try
            {
                IQueryable<AccessAudit> retrievedAccessAudits =
                    await this.accessAuditService.RetrieveAllAccessAuditsAsync();

                return Ok(retrievedAccessAudits);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                return InternalServerError(accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                return InternalServerError(accessAuditServiceException);
            }
        }

        [HttpGet("{accessAuditId}")]
        public async ValueTask<ActionResult<AccessAudit>> GetAccessAuditByIdAsync(Guid accessAuditId)
        {
            try
            {
                AccessAudit accessAudit = await this.accessAuditService.RetrieveAccessAuditByIdAsync(accessAuditId);

                return Ok(accessAudit);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
                when (accessAuditValidationException.InnerException is NotFoundAccessAuditException)
            {
                return NotFound(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
            {
                return BadRequest(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
            {
                return BadRequest(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                return InternalServerError(accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                return InternalServerError(accessAuditServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<AccessAudit>> PutAccessAuditAsync(AccessAudit accessAudit)
        {
            try
            {
                AccessAudit modifiedAccessAudit =
                    await this.accessAuditService.ModifyAccessAuditAsync(accessAudit);

                return Ok(modifiedAccessAudit);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
                when (accessAuditValidationException.InnerException is NotFoundAccessAuditException)
            {
                return NotFound(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
            {
                return BadRequest(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
               when (accessAuditDependencyValidationException.InnerException is AlreadyExistsAccessAuditException)
            {
                return Conflict(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
            {
                return BadRequest(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                return InternalServerError(accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                return InternalServerError(accessAuditServiceException);
            }
        }

        [HttpDelete("{accessAuditId}")]
        public async ValueTask<ActionResult<AccessAudit>> DeleteAccessAuditByIdAsync(Guid accessAuditId)
        {
            try
            {
                AccessAudit deletedAccessAudit =
                    await this.accessAuditService.RemoveAccessAuditByIdAsync(accessAuditId);

                return Ok(deletedAccessAudit);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
                when (accessAuditValidationException.InnerException is NotFoundAccessAuditException)
            {
                return NotFound(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditValidationException accessAuditValidationException)
            {
                return BadRequest(accessAuditValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
                when (accessAuditDependencyValidationException.InnerException is LockedAccessAuditException)
            {
                return Locked(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyValidationException accessAuditDependencyValidationException)
            {
                return BadRequest(accessAuditDependencyValidationException.InnerException);
            }
            catch (AccessAuditDependencyException accessAuditDependencyException)
            {
                return InternalServerError(accessAuditDependencyException);
            }
            catch (AccessAuditServiceException accessAuditServiceException)
            {
                return InternalServerError(accessAuditServiceException);
            }
        }
    }
}
