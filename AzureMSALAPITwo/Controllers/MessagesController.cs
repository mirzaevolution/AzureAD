using Microsoft.AspNetCore.Mvc;

namespace AzureMSALAPITwo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        public string Get() => "Holla! You get message from Endpoint 2!";
    }
}