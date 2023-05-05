using AutoMapper;
using Custos.database.models;
using Custos.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Custos.Controllers;

[ApiController]
[Route("/ctf")]
public class CtfController : BaseApiController
{
    #region Special Data

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

    public class GetCtfsRequest
    {
        public int Offset { get; init; }
        public int Limit { get; init; }
    }

    #endregion

    private readonly CtfLogic _ctfLogic;
    private readonly IMapper _mapper;

    public CtfController(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _ctfLogic = Scope.ServiceProvider.GetRequiredService<CtfLogic>();
        _mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    [HttpGet]
    [Route("")]
    public override IActionResult OnRoot()
    {
        return base.OnRoot();
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

    [HttpGet]
    [Route("get/{ctfUid}")]
    public IActionResult OnGet(Guid ctfUid)
    {
        var result = _ctfLogic.GetCtf(ctfUid);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpGet]
    [Route("get")]
    public IActionResult OnGet([FromQuery] GetCtfsRequest request)
    {
        var result = _ctfLogic.GetCtfs(request.Offset, request.Limit);
        return GenericHttpResponse.FromLogicResult(result);
    }
}