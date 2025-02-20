﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Attrify.Attributes;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
    [ApiController]
    [Route("api/[controller]")]
    public class PdsDataController : RESTFulController
    {
        private readonly IPdsDataService pdsDataService;

        public PdsDataController(IPdsDataService pdsDataService) =>
            this.pdsDataService = pdsDataService;

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPost]
        public async ValueTask<ActionResult<PdsData>> PostPdsDataAsync([FromBody] PdsData pdsData)
        {
            try
            {
                PdsData addedPdsData =
                    await this.pdsDataService.AddPdsDataAsync(pdsData);

                return Created(addedPdsData);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                return BadRequest(pdsDataValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
               when (pdsDataDependencyValidationException.InnerException is AlreadyExistsPdsDataException)
            {
                return Conflict(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
            {
                return BadRequest(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [InvisibleApi]
        [HttpGet]
#if !DEBUG
        [EnableQuery(PageSize = 50)]
#endif
#if DEBUG
        [EnableQuery(PageSize = 5000)]
#endif
        public async ValueTask<ActionResult<IQueryable<PdsData>>> Get()
        {
            try
            {
                IQueryable<PdsData> retrievedPdsDatas = await this.pdsDataService.RetrieveAllPdsDatasAsync();

                return Ok(retrievedPdsDatas);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [InvisibleApi]
        [HttpGet("{pdsDataId}")]
        public async ValueTask<ActionResult<PdsData>> GetPdsDataByIdAsync(Guid pdsDataId)
        {
            try
            {
                PdsData retrievedPdsData = await this.pdsDataService.RetrievePdsDataByIdAsync(pdsDataId);

                return Ok(retrievedPdsData);
            }
            catch (PdsDataValidationException pdsDataValidationException)
                when (pdsDataValidationException.InnerException is NotFoundPdsDataException)
            {
                return NotFound(pdsDataValidationException.InnerException);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                return BadRequest(pdsDataValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
            {
                return BadRequest(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<PdsData>> PutPdsDataAsync([FromBody] PdsData pdsData)
        {
            try
            {
                PdsData modifiedPdsData =
                    await this.pdsDataService.ModifyPdsDataAsync(pdsData);

                return Ok(modifiedPdsData);
            }
            catch (PdsDataValidationException pdsDataValidationException)
                when (pdsDataValidationException.InnerException is NotFoundPdsDataException)
            {
                return NotFound(pdsDataValidationException.InnerException);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                return BadRequest(pdsDataValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
               when (pdsDataDependencyValidationException.InnerException is AlreadyExistsPdsDataException)
            {
                return Conflict(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
            {
                return BadRequest(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }

        [Authorize(Roles = "ISL.Reidentification.Portal.Administrators")]
        [InvisibleApi]
        [HttpDelete("{pdsDataId}")]
        public async ValueTask<ActionResult<PdsData>> DeletePdsDataByIdAsync(Guid pdsDataId)
        {
            try
            {
                PdsData pdsData = await this.pdsDataService.RemovePdsDataByIdAsync(pdsDataId);

                return Ok(pdsData);
            }
            catch (PdsDataValidationException pdsDataValidationException)
                when (pdsDataValidationException.InnerException is NotFoundPdsDataException)
            {
                return NotFound(pdsDataValidationException.InnerException);
            }
            catch (PdsDataValidationException pdsDataValidationException)
            {
                return BadRequest(pdsDataValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
                when (pdsDataDependencyValidationException.InnerException is LockedPdsDataException)
            {
                return Locked(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyValidationException pdsDataDependencyValidationException)
            {
                return BadRequest(pdsDataDependencyValidationException.InnerException);
            }
            catch (PdsDataDependencyException pdsDataDependencyException)
            {
                return InternalServerError(pdsDataDependencyException);
            }
            catch (PdsDataServiceException pdsDataServiceException)
            {
                return InternalServerError(pdsDataServiceException);
            }
        }
    }
}
