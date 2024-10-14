// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReIdentificationController : RESTFulController
    {
        private readonly IIdentificationCoordinationService identificationCoordinationService;
        private readonly ICsvIdentificationRequestService csvIdentificationRequestService;

        public ReIdentificationController(IIdentificationCoordinationService identificationCoordinationService,
                ICsvIdentificationRequestService csvIdentificationRequestService)
        {
            this.identificationCoordinationService = identificationCoordinationService;
            this.csvIdentificationRequestService = csvIdentificationRequestService;
        }


        [HttpPost]
        public async ValueTask<ActionResult<AccessRequest>>
            PostIdentificationRequestsAsync([FromBody] AccessRequest accessRequest)
        {
            try
            {
                AccessRequest addedAccessRequest =
                    await this.identificationCoordinationService.ProcessIdentificationRequestsAsync(accessRequest);

                return Created(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (
                IdentificationCoordinationDependencyValidationException
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

        [HttpPost("csv")]
        public async ValueTask<ActionResult<AccessRequest>>
            PostCsvIdentificationRequestAsync([FromBody] AccessRequest accessRequest)
        {
            try
            {
                AccessRequest addedAccessRequest =
                    await this.identificationCoordinationService.ProcessCsvIdentificationRequestAsync(accessRequest);

                return Created(addedAccessRequest);
            }
            catch (IdentificationCoordinationValidationException identificationCoordinationValidationException)
            {
                return BadRequest(identificationCoordinationValidationException.InnerException);
            }
            catch (
                IdentificationCoordinationDependencyValidationException
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

        [HttpGet("{csvIdentificationRequestId}")]
        public async ValueTask<ActionResult> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId)
        {
            CsvIdentificationRequest csvIdentificationRequest =
                await this.csvIdentificationRequestService
                    .RetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            AccessRequest accessRequest = new AccessRequest();
            accessRequest.CsvIdentificationRequest = csvIdentificationRequest;

            AccessRequest reIdentifiedAccessRequest =
                await this.identificationCoordinationService.ReIdentifyCsvIdentificationRequestAsync(accessRequest);

            string fileName = "data.csv";
            string contentType = "text/csv";

            return File(csvIdentificationRequest.Data, contentType, fileName);
        }
    }
}
