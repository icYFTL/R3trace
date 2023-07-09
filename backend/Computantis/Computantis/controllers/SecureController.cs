using Computantis.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.attributes;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Computantis.controllers;

[ApiController]
[Route("/secure")]
public class SecureController : BaseApiController
{
    #region Special Data

    public class CheckTokenRequest
    {
        public string Token { get; init; } = null!;
    }

    public class TeamDeleteRequest
    {
        public Guid TeamUid { get; init; }
    }

    #endregion

    private readonly TeamsLogic _teamsLogic;
    private readonly UsersLogic _usersLogic;
    private readonly ILogger<SecureController> _logger;

    public SecureController(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory) : base(scopeFactory)
    {
        _teamsLogic = Scope.ServiceProvider.GetRequiredService<TeamsLogic>();
        _usersLogic = Scope.ServiceProvider.GetRequiredService<UsersLogic>();
        _logger = loggerFactory.CreateLogger<SecureController>();
    }
    
    [ForAdmin]
    [HttpGet]
    [Route("")]
    public override IActionResult OnRoot()
    {
        return base.OnRoot();
    }
    
    [ForAdmin]
    [HttpDelete]
    [Route("team/delete")]
    public IActionResult OnDelete([FromBody] TeamDeleteRequest request)
    {
        var result = _teamsLogic.DeleteTeam(request.TeamUid);
        return GenericHttpResponse.FromLogicResult(result);
    }
    
    [ForAdmin]
    [HttpDelete]
    [Route("team/update")]
    public IActionResult OnUpdate([FromBody] TeamDeleteRequest request)
    {
        var result = _teamsLogic.DeleteTeam(request.TeamUid);
        return GenericHttpResponse.FromLogicResult(result);
    }
    
    // NGINX INTERNAL ONLY
    [HttpPost]
    [Route("user/check_token")]
    public IActionResult OnCheckToken()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var result = _usersLogic.CheckToken(token);
        return GenericHttpResponse.FromLogicResult(result);
    }
}