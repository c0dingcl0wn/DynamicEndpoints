using DynamicEndpoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace DynamicEndpoints.Controllers;
[ApiController]
    [Route("[controller]")]
    public class EndpointGeneratorController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] GenerationOptions generationOptions)
        {
            return Ok(generationOptions);
        }
    }
