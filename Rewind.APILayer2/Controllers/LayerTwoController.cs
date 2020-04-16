using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rewind.APILayer2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerTwoController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Middleware.Read")]
        [HttpGet("GetMessage")]
        public string GetMessage()
        {
            return "Holla! You get message from secondary api endpoint";
        }
    }
}