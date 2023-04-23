using Microsoft.Extensions.DependencyInjection;

namespace R3TraceShared.logic;

public abstract class BaseLogic
{
    protected readonly IServiceScope Scope;
    protected BaseLogic(IServiceScopeFactory scopeFactory)
    {
        Scope = scopeFactory.CreateScope();
    }
}