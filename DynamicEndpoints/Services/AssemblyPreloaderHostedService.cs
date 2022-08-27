using DynamicEndpoints.Providers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DynamicEndpoints.Services;

public class AssemblyPreloaderHostedService : IHostedService
{
    private readonly ApplicationPartManager _partManager;
    public AssemblyPreloaderHostedService(ApplicationPartManager partManager)
    {
        _partManager = partManager;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var assemblies = AssemblyProvider.GetAllGeneratedAssemblies();
        foreach (var assembly in assemblies)
        {
            _partManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }

        ActionDescriptorChangeProvider.Instance.HasChanged = true;
        ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}