// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [Authorize(Roles = "Administrators")]
    [ApiController]
    [Route("api/[controller]")]
    public class CsvIdentificationRequestsController : RESTFulController
    {
        private readonly ICsvIdentificationRequestService csvIdentificationRequestService;

        public CsvIdentificationRequestsController(ICsvIdentificationRequestService csvIdentificationRequestService) =>
            this.csvIdentificationRequestService = csvIdentificationRequestService;

        [HttpPost]
        public async ValueTask<ActionResult<CsvIdentificationRequest>> PostCsvIdentificationRequestAsync(
            [FromBody] CsvIdentificationRequest csvIdentificationRequest)
        {
            try
            {
                CsvIdentificationRequest addedCsvIdentificationRequest =
                    await this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(
                        csvIdentificationRequest);

                return Created(addedCsvIdentificationRequest);
            }
            catch (CsvIdentificationRequestValidationException lookupValidationException)
            {
                return BadRequest(lookupValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException lookupDependencyValidationException)
                when (lookupDependencyValidationException.InnerException
                    is AlreadyExistsCsvIdentificationRequestException)
            {
                return Conflict(lookupDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException lookupDependencyValidationException)
            {
                return BadRequest(lookupDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyException lookupDependencyException)
            {
                return InternalServerError(lookupDependencyException);
            }
            catch (CsvIdentificationRequestServiceException lookupServiceException)
            {
                return InternalServerError(lookupServiceException);
            }
        }

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public async ValueTask<ActionResult<IQueryable<CsvIdentificationRequest>>> Get()
        {
            try
            {
                IQueryable<CsvIdentificationRequest> csvIdentificationRequests =
                    await this.csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync();

                return Ok(csvIdentificationRequests);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                return InternalServerError(csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                return InternalServerError(csvIdentificationRequestServiceException);
            }
        }

        [HttpGet("{csvIdentificationRequestId}")]
        public async ValueTask<ActionResult<CsvIdentificationRequest>> GetCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId)
        {
            try
            {
                CsvIdentificationRequest csvIdentificationRequest =
                    await this.csvIdentificationRequestService.RetrieveCsvIdentificationRequestByIdAsync(
                        csvIdentificationRequestId);

                return Ok(csvIdentificationRequest);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
                when (csvIdentificationRequestValidationException.InnerException
                    is NotFoundCsvIdentificationRequestException)
            {
                return NotFound(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
            {
                return BadRequest(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
            {
                return BadRequest(csvIdentificationRequestDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                return InternalServerError(csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                return InternalServerError(csvIdentificationRequestServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<CsvIdentificationRequest>> PutCsvIdentificationRequestAsync(
            [FromBody] CsvIdentificationRequest csvIdentificationRequest)
        {
            try
            {
                CsvIdentificationRequest modifiedCsvIdentificationRequest =
                    await this.csvIdentificationRequestService
                        .ModifyCsvIdentificationRequestAsync(csvIdentificationRequest);

                return Ok(modifiedCsvIdentificationRequest);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
                when (csvIdentificationRequestValidationException.InnerException
                    is NotFoundCsvIdentificationRequestException)
            {
                return NotFound(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
            {
                return BadRequest(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
                    when (csvIdentificationRequestDependencyValidationException.InnerException
                        is AlreadyExistsCsvIdentificationRequestException)
            {
                return Conflict(csvIdentificationRequestDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
            {
                return BadRequest(csvIdentificationRequestDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                return InternalServerError(csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                return InternalServerError(csvIdentificationRequestServiceException);
            }
        }

        [HttpDelete("{csvIdentificationRequestId}")]
        public async ValueTask<ActionResult<CsvIdentificationRequest>> DeleteCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId)
        {
            try
            {
                CsvIdentificationRequest deletedCsvIdentificationRequest =
                    await this.csvIdentificationRequestService
                        .RemoveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

                return Ok(deletedCsvIdentificationRequest);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
                when (csvIdentificationRequestValidationException.InnerException
                    is NotFoundCsvIdentificationRequestException)
            {
                return NotFound(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestValidationException csvIdentificationRequestValidationException)
            {
                return BadRequest(csvIdentificationRequestValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
                    when (csvIdentificationRequestDependencyValidationException.InnerException
                        is LockedCsvIdentificationRequestException)
            {
                return Locked(csvIdentificationRequestDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyValidationException
                csvIdentificationRequestDependencyValidationException)
            {
                return BadRequest(csvIdentificationRequestDependencyValidationException.InnerException);
            }
            catch (CsvIdentificationRequestDependencyException csvIdentificationRequestDependencyException)
            {
                return InternalServerError(csvIdentificationRequestDependencyException);
            }
            catch (CsvIdentificationRequestServiceException csvIdentificationRequestServiceException)
            {
                return InternalServerError(csvIdentificationRequestServiceException);
            }
        }
    }
}
