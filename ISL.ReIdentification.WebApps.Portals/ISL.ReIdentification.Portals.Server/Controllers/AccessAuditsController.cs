﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccessAuditsController : RESTFulController
    {
        private readonly IAccessAuditService accessAuditService;

        public AccessAuditsController(IAccessAuditService accessAuditService) =>
            this.accessAuditService = accessAuditService;

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPost]
        public async ValueTask<ActionResult<AccessAudit>> PostAccessAuditAsync([FromBody] AccessAudit accessAudit)
        {
            try
            {
                AccessAudit addedAccessAudit =
                    await accessAuditService.AddAccessAuditAsync(accessAudit);

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
        [EnableQuery]
        public async ValueTask<ActionResult<IQueryable<AccessAudit>>> Get()
        {
            try
            {
                IQueryable<AccessAudit> retrievedAccessAudits =
                    await accessAuditService.RetrieveAllAccessAuditsAsync();

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
                AccessAudit accessAudit = await accessAuditService.RetrieveAccessAuditByIdAsync(accessAuditId);

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

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<AccessAudit>> PutAccessAuditAsync([FromBody] AccessAudit accessAudit)
        {
            try
            {
                AccessAudit modifiedAccessAudit =
                    await accessAuditService.ModifyAccessAuditAsync(accessAudit);

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

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpDelete("{accessAuditId}")]
        public async ValueTask<ActionResult<AccessAudit>> DeleteAccessAuditByIdAsync(Guid accessAuditId)
        {
            try
            {
                AccessAudit deletedAccessAudit =
                    await accessAuditService.RemoveAccessAuditByIdAsync(accessAuditId);

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
