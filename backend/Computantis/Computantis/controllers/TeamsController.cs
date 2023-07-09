using Computantis.database.models;
using Computantis.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Computantis.controllers;

[ApiController]
[Route("/team")]
public class TeamsController : BaseApiController
{
    #region Special Data

    public class TeamCreateRequest
    {
        public string Name { get; init; } = null!;
    }

    public class TeamUpdateRequest
    {
        public string? Name { get; init; }
        public bool? Code { get; init; }
        public Guid? OwnerUid { get; init; }
    }

    public class TeamGetsRequest
    {
        public int Limit { get; init; }
        public int Offset { get; init; }
    }

    public class TeamJoinRequest
    {
        public string Code { get; init; } = null!;
    }

    public class TeamDelegateRequest
    {
        public Guid NewOwnerUid { get; init; }
    }

    #endregion

    private readonly TeamsLogic _teamsLogic;

    public TeamsController(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _teamsLogic = Scope.ServiceProvider.GetRequiredService<TeamsLogic>();
    }

    [HttpGet]
    [Route("")]
    public override IActionResult OnRoot()
    {
        var result = _teamsLogic.GetMyTeam();
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPut]
    [Route("create")]
    public IActionResult OnCreate([FromBody] TeamCreateRequest request)
    {
        var result = _teamsLogic.CreateTeam(new Team
        {
            Name = request.Name
        });

        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("update")]
    public IActionResult OnUpdate([FromBody] TeamUpdateRequest request)
    {
        var result =
            _teamsLogic.UpdateTeam(request.Name, request.Code, request.OwnerUid);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("join")]
    public IActionResult OnJoin([FromBody] TeamJoinRequest request)
    {
        var result =
            _teamsLogic.JoinTeam(request.Code);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("leave")]
    public IActionResult OnLeave()
    {
        var result =
            _teamsLogic.LeaveTeam();
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("delegate")]
    public IActionResult OnDelegate([FromBody] TeamDelegateRequest request)
    {
        var result =
            _teamsLogic.DelegateTeam(request.NewOwnerUid);
        return GenericHttpResponse.FromLogicResult(result);
    }


    [HttpGet]
    [Route("get/{teamUid}")]
    public IActionResult OnGet(Guid teamUid)
    {
        var result = _teamsLogic.GetTeam(teamUid);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpGet]
    [Route("get")]
    public IActionResult OnGet([FromQuery] TeamGetsRequest request)
    {
        var result = _teamsLogic.GetTeams(request.Limit, request.Offset);
        return GenericHttpResponse.FromLogicResult(result);
    }
}