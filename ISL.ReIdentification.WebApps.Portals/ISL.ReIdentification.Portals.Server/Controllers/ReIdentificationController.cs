﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReIdentificationController : RESTFulController
    {
        private readonly IIdentificationCoordinationService identificationCoordinationService;

        public ReIdentificationController(IIdentificationCoordinationService identificationCoordinationService)
        {
            this.identificationCoordinationService = identificationCoordinationService;
        }

        [HttpPost]
        public async ValueTask<ActionResult<AccessRequest>> PostIdentificationRequestsAsync(
            [FromBody] AccessRequest accessRequest)
        {
            try
            {
                AccessRequest addedAccessRequest = await identificationCoordinationService
                    .ProcessIdentificationRequestsAsync(accessRequest);

                return Created(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
                when (identificationCoordinationDependencyValidationException.InnerException
                    is UnauthorizedIdentificationCoordinationException)
            {
                return Unauthorized(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }

        [HttpPost("submitcsv")]
        public async ValueTask<ActionResult<AccessRequest>> PostCsvIdentificationRequestAsync(
            [FromBody] AccessRequest accessRequest)
        {
            try
            {
                AccessRequest addedAccessRequest = await identificationCoordinationService
                    .PersistsCsvIdentificationRequestAsync(accessRequest);

                return Created(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators,ISL.Reidentification.Portal.DataEngineers")]
        [HttpPost("impersonation")]
        public async ValueTask<ActionResult<AccessRequest>> PostImpersonationContextRequestAsync(
            [FromBody] AccessRequest accessRequest)
        {
            try
            {
                AccessRequest addedAccessRequest = await identificationCoordinationService
                    .PersistsImpersonationContextAsync(accessRequest);

                return Created(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators,ISL.Reidentification.Portal.DataEngineers")]
        [HttpGet("generatetokens/{impersonationContextId}")]
        public async ValueTask<ActionResult<AccessRequest>> PostImpersonationContextGenerateTokensAsync(
             Guid impersonationContextId)
        {
            try
            {
                AccessRequest addedAccessRequest = await identificationCoordinationService
                    .ExpireRenewImpersonationContextTokensAsync(impersonationContextId);

                return Ok(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }

        [Authorize]
        [HttpPost("impersonationcontextapproval")]
        public async ValueTask<ActionResult> PostImpersonationContextApprovalAsync(
            [FromBody] ApprovalRequest approvalRequest)
        {
            try
            {
                await identificationCoordinationService.ImpersonationContextApprovalAsync(
                    approvalRequest.ImpersonationContextId,
                    approvalRequest.IsApproved);

                return Ok();
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }

        [HttpGet("csvreidentification/{csvIdentificationRequestId}/{reason}")]
        public async ValueTask<ActionResult> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId, string reason)
        {
            try
            {
                AccessRequest reIdentifiedAccessRequest = await identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(csvIdentificationRequestId, reason);

                string contentType = "text/csv";

                return File(
                    reIdentifiedAccessRequest.CsvIdentificationRequest.Data,
                    contentType,
                    reIdentifiedAccessRequest.CsvIdentificationRequest.Filepath);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
                when (identificationCoordinationDependencyValidationException.InnerException
                    is UnauthorizedIdentificationCoordinationException)
            {
                return Unauthorized(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyValidationException
                identificationCoordinationDependencyValidationException)
            {
                return BadRequest(identificationCoordinationDependencyValidationException.InnerException);
            }
            catch (IdentificationCoordinationDependencyException identificationCoordinationDependencyException)
            {
                return InternalServerError(identificationCoordinationDependencyException);
            }
            catch (IdentificationCoordinationServiceException identificationCoordinationServiceException)
            {
                return InternalServerError(identificationCoordinationServiceException);
            }
        }
    }
}
