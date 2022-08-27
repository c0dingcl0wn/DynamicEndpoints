using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace DynamicEndpoints.Providers;

public class ActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
   
    public static ActionDescriptorChangeProvider Instance { get; } = new ();

    public CancellationTokenSource TokenSource { get; private set; }

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken()
    {
        TokenSource = new CancellationTokenSource();
        return new CancellationChangeToken(TokenSource.Token);
    }
    
}