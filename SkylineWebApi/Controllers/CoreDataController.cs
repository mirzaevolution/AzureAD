using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SkylineWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreDataController : ControllerBase
    {
        [Authorize(Policy = "ClaimAuthorization")]
        public IActionResult Get()
        {

            return Ok(new { message = "Hello World!" });
        }
    }
}