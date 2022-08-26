using System.Text.Json.Serialization;

namespace DynamicEndpoints.Model;

public class GenerationOptions
{
    public string Code { get; set;  }
    public string ControllerName { get; set; }
    public string ControllerRoute { get; set; }
}