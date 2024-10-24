﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;

namespace ISL.ReIdentification.Portals.Server.Controllers
{
    [Authorize(Roles = "Administrators")]
    [ApiController]
    [Route("api/[controller]")]
    public class OdsDataController : RESTFulController
    {
        private readonly IOdsDataService odsDataService;

        public OdsDataController(IOdsDataService odsDataService) =>
            this.odsDataService = odsDataService;

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public async ValueTask<ActionResult<IQueryable<OdsData>>> Get()
        {
            try
            {
                IQueryable<OdsData> retrievedOdsDatas = await odsDataService.RetrieveAllOdsDatasAsync();

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("GetChildren")]
        public async ValueTask<ActionResult<List<OdsData>>> GetAllChildren(Guid id)
        {
            try
            {
                List<OdsData> retrievedOdsDatas = await odsDataService.RetrieveChildrenByParentId(id);

                return Ok(retrievedOdsDatas);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }

        [HttpGet("{odsDataId}")]
        public async ValueTask<ActionResult<OdsData>> GetOdsDataByIdAsync(Guid odsDataId)
        {
            try
            {
                OdsData retrievedOdsData = await odsDataService.RetrieveOdsDataByIdAsync(odsDataId);

                return Ok(retrievedOdsData);
            }
            catch (OdsDataValidationException odsDataValidationException)
                when (odsDataValidationException.InnerException is NotFoundOdsDataException)
            {
                return NotFound(odsDataValidationException.InnerException);
            }
            catch (OdsDataValidationException odsDataValidationException)
            {
                return BadRequest(odsDataValidationException.InnerException);
            }
            catch (OdsDataDependencyException odsDataDependencyException)
            {
                return InternalServerError(odsDataDependencyException);
            }
            catch (OdsDataServiceException odsDataServiceException)
            {
                return InternalServerError(odsDataServiceException);
            }
        }
    }
}
