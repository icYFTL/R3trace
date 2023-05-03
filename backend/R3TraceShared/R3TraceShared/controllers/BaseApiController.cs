using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using R3TraceShared.http.response.generic;

namespace R3TraceShared.controllers;

public class BaseApiController : ControllerBase
{
    protected readonly IServiceScope Scope;

    public BaseApiController(IServiceScopeFactory scopeFactory)
    {
        Scope = scopeFactory.CreateScope();
    }

    public virtual IActionResult OnRoot()
    {
        return new SuccessHttpResponse();
    }
}