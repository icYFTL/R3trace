using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace R3TraceShared.communicators;

public abstract class BaseCommunicator
{
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly IConfiguration Configuration;

    public BaseCommunicator(IServiceScopeFactory scopeFactory)
    {
        Client = new HttpClient();
        Scope = scopeFactory.CreateScope();
        Configuration = Scope.ServiceProvider.GetRequiredService<IConfiguration>();
    }
}