using AutoMapper;
using Custos.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Custos.сontrollers;

[ApiController]
[Route("/ctf")]
public class CtfController : BaseApiController
{
    #region Special Data
    public class GetCtfsRequest
    {
        public int Offset { get; init; }
        public int Limit { get; init; }
    }

    public class JoinCtfRequest
    {
        public Guid CtfUid { get; init; }
        public string? Code { get; init; }
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
    
    [HttpPost]
    [Route("join")]
    public IActionResult OnJoin([FromBody] JoinCtfRequest request)
    {
        var result = _ctfLogic.JoinCtf(request.CtfUid, request.Code);
        return GenericHttpResponse.FromLogicResult(result);
    }
}