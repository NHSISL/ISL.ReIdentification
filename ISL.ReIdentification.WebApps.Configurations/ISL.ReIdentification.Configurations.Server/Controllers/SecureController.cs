// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ISL.ReIdentification.Configurations.Server.Controllers
{
    [Authorize(Roles = "NoAccess")]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureController : Controller
    {
        private readonly IConfiguration configuration;

        public SecureController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult GetFeatures()
        {
            return Ok("you'll never get me");
        }
    }
}
