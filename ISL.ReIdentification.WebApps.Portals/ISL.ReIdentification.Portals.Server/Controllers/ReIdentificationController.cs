// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize(Roles = "Administrators")]
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
        public async ValueTask<ActionResult<AccessRequest>>
            PostIdentificationRequestsAsync([FromBody] AccessRequest accessRequest)
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
        public async ValueTask<ActionResult<AccessRequest>>
            PostCsvIdentificationRequestAsync([FromBody] AccessRequest accessRequest)
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

        [HttpPost("impersonation")]
        public async ValueTask<ActionResult<AccessRequest>>
            PostImpersonationContextRequestAsync([FromBody] AccessRequest accessRequest)
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

        [HttpGet("{csvreIdentification}")]
        public async ValueTask<ActionResult> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId)
        {
            try
            {
                AccessRequest reIdentifiedAccessRequest = await identificationCoordinationService
                    .ProcessCsvIdentificationRequestAsync(csvIdentificationRequestId);

                string fileName = "data.csv";
                string contentType = "text/csv";

                return File(reIdentifiedAccessRequest.CsvIdentificationRequest.Data, contentType, fileName);
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
    }
}
