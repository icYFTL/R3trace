using AutoMapper;
using Custos.database.models;
using Custos.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.attributes;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Custos.сontrollers;

[ForAdmin]
[ApiController]
[Route("/secure")]
public class SecureController : BaseApiController
{
    public class CreateCtfRequest
    {
        public string Name { get; init; } = null!;
        public string Title { get; init; } = null!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string? Code { get; init; }
    }

    public class UpdateCtfRequest : CreateCtfRequest
    {
        public Guid Uid { get; init; }
    }

    public class DeleteCtfRequest
    {
        public Guid CtfUid { get; init; }
    }
    
    private readonly CtfLogic _ctfLogic;
    private readonly IMapper _mapper;

    public SecureController(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _ctfLogic = Scope.ServiceProvider.GetRequiredService<CtfLogic>();
        _mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    [HttpPut]
    [Route("create")]
    public IActionResult OnCreate([FromBody] CreateCtfRequest request)
    {
        var result = _ctfLogic.CreateCtf(_mapper.Map<Ctf>(request));
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("update")]
    public IActionResult OnUpdate([FromBody] UpdateCtfRequest request)
    {
        var result = _ctfLogic.UpdateCtf(_mapper.Map<Ctf>(request));
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult OnDelete([FromBody] DeleteCtfRequest request)
    {
        var result = _ctfLogic.DeleteCtf(request.CtfUid);
        return GenericHttpResponse.FromLogicResult(result);
    }
}