using DynamicEndpoints.Model;
using DynamicEndpoints.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DynamicEndpoints.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class EndpointGeneratorController : ControllerBase
    {
        private readonly ApplicationPartManager _partManager;

        public EndpointGeneratorController(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] GenerationOptions generationOptions)
        {
            var assembly = AssemblyProvider.CreateOrGetAssembly(generationOptions, out var diagnostics);
        
            // add assembly to application parts (this adds a new controller in our case)
            if (assembly == null)
            {
                var errors = string.Join("\n", diagnostics.Select(diagnostic => diagnostic.ToString() ));
                return BadRequest(errors);
            }
            _partManager.ApplicationParts.Add(new AssemblyPart(assembly));

            // notify ASP net of the changes
            ActionDescriptorChangeProvider.Instance.HasChanged = true;
            ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        
            return Ok($"Created {generationOptions.ControllerName}\nNavigate to {generationOptions.ControllerRoute} to see it in action.");        
        }
    }
