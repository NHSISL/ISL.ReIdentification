﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.Audits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    //[Authorize(Roles = "ISL.Reidentification.Configuration.Administrators")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditsController : RESTFulController
    {
        private readonly IAuditService auditService;

        public AuditsController(IAuditService auditService) =>
            this.auditService = auditService;

        [InvisibleApi]
        [HttpPost]
        public async ValueTask<ActionResult<Audit>> PostAuditAsync([FromBody] Audit audit)
        {
            try
            {
                Audit addedAudit =
                    await this.auditService.AddAuditAsync(audit);

                return Created(addedAudit);
            }
            catch (AuditValidationException auditValidationException)
            {
                return BadRequest(auditValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
               when (auditDependencyValidationException.InnerException is AlreadyExistsAuditException)
            {
                return Conflict(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                return BadRequest(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                return InternalServerError(auditDependencyException);
            }
            catch (AuditServiceException auditServiceException)
            {
                return InternalServerError(auditServiceException);
            }
        }

        [HttpGet]
        [EnableQuery()]
        public async ValueTask<ActionResult<IQueryable<Audit>>> Get()
        {
            try
            {
                IQueryable<Audit> retrievedAudits =
                    await this.auditService.RetrieveAllAuditsAsync();

                return Ok(retrievedAudits);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                return InternalServerError(auditDependencyException);
            }
            catch (AuditServiceException auditServiceException)
            {
                return InternalServerError(auditServiceException);
            }
        }

        [InvisibleApi]
        [HttpGet("{auditId}")]
        public async ValueTask<ActionResult<Audit>> GetAuditByIdAsync(Guid auditId)
        {
            try
            {
                Audit audit = await this.auditService.RetrieveAuditByIdAsync(auditId);

                return Ok(audit);
            }
            catch (AuditValidationException auditValidationException)
                when (auditValidationException.InnerException is NotFoundAuditException)
            {
                return NotFound(auditValidationException.InnerException);
            }
            catch (AuditValidationException auditValidationException)
            {
                return BadRequest(auditValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                return BadRequest(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                return InternalServerError(auditDependencyException);
            }
            catch (AuditServiceException auditServiceException)
            {
                return InternalServerError(auditServiceException);
            }
        }

        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<Audit>> PutAuditAsync([FromBody] Audit audit)
        {
            try
            {
                Audit modifiedAudit =
                    await this.auditService.ModifyAuditAsync(audit);

                return Ok(modifiedAudit);
            }
            catch (AuditValidationException auditValidationException)
                when (auditValidationException.InnerException is NotFoundAuditException)
            {
                return NotFound(auditValidationException.InnerException);
            }
            catch (AuditValidationException auditValidationException)
            {
                return BadRequest(auditValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
               when (auditDependencyValidationException.InnerException is AlreadyExistsAuditException)
            {
                return Conflict(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                return BadRequest(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                return InternalServerError(auditDependencyException);
            }
            catch (AuditServiceException auditServiceException)
            {
                return InternalServerError(auditServiceException);
            }
        }

        [InvisibleApi]
        [HttpDelete("{auditId}")]
        public async ValueTask<ActionResult<Audit>> DeleteAuditByIdAsync(Guid auditId)
        {
            try
            {
                Audit deletedAudit =
                    await this.auditService.RemoveAuditByIdAsync(auditId);

                return Ok(deletedAudit);
            }
            catch (AuditValidationException auditValidationException)
                when (auditValidationException.InnerException is NotFoundAuditException)
            {
                return NotFound(auditValidationException.InnerException);
            }
            catch (AuditValidationException auditValidationException)
            {
                return BadRequest(auditValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
                when (auditDependencyValidationException.InnerException is LockedAuditException)
            {
                return Locked(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyValidationException auditDependencyValidationException)
            {
                return BadRequest(auditDependencyValidationException.InnerException);
            }
            catch (AuditDependencyException auditDependencyException)
            {
                return InternalServerError(auditDependencyException);
            }
            catch (AuditServiceException auditServiceException)
            {
                return InternalServerError(auditServiceException);
            }
        }
    }
}
