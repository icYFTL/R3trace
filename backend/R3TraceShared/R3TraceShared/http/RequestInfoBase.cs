using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace R3TraceShared.http;

public class RequestInfoBase
{
    protected readonly IServiceScope Scope;
    protected readonly HttpContext HttpContext;

    public string RemoteIp = null!;
    public string ApiVersion = null!;

    public RequestInfoBase(IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory)
    {
        Scope = scopeFactory.CreateScope();
        HttpContext = contextAccessor.HttpContext!;

        Init();
    }

    protected virtual void Init()
    {
        throw new NotImplementedException();
    }

    public virtual void Reload()
    {
        throw new NotImplementedException();
    }
}