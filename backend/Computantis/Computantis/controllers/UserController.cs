using Computantis.database.models;
using Computantis.logic;
using Microsoft.AspNetCore.Mvc;
using R3TraceShared.controllers;
using R3TraceShared.http.response.generic;

namespace Computantis.controllers;

[ApiController]
[Route("/user")]
public class UserController : BaseApiController
{
    #region Special Data

    public class UserSignUpRequest
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public Guid? NationalityUid { get; init; }
    }

    public class UserSignInRequest
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    public class UserUpdateRequest
    {
        public string? Username { get; init; }
        public string? Password { get; init; }
        public Guid? NationalityUid { get; init; }
        public Guid? TeamUid { get; init; }
    }

    public class UserGetsRequest
    {
        public int Limit { get; init; }
        public int Offset { get; init; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; init; }
    }

    #endregion

    private readonly UsersLogic _usersLogic;

    public UserController(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _usersLogic = Scope.ServiceProvider.GetRequiredService<UsersLogic>();
    }

    [HttpGet]
    [Route("")]
    public override IActionResult OnRoot()
    {
        var result = _usersLogic.GetMe();
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPut]
    [Route("sign_up")]
    public IActionResult OnSignUp([FromBody] UserSignUpRequest request)
    {
        var result = _usersLogic.CreateUser(new User
        {
            Username = request.Username,
            Password = request.Password,
            NationalityUid = request.NationalityUid
        });

        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("sign_in")]
    public IActionResult OnSignIn([FromBody] UserSignInRequest request)
    {
        var result = _usersLogic.SignIn(request.Username, request.Password);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpPost]
    [Route("update")]
    public IActionResult OnUpdate([FromBody] UserUpdateRequest request)
    {
        var result =
            _usersLogic.UpdateUser(request.Username, request.Password, request.NationalityUid,
                request.TeamUid);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult OnDelete()
    {
        var result = _usersLogic.DeleteUser();
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpGet]
    [Route("get/{userUid}")]
    public IActionResult OnGet(Guid userUid)
    {
        var result = _usersLogic.GetUser(userUid);
        return GenericHttpResponse.FromLogicResult(result);
    }

    [HttpGet]
    [Route("get")]
    public IActionResult OnGet([FromQuery] UserGetsRequest request)
    {
        var result = _usersLogic.GetUsers(request.Limit, request.Offset);
        return GenericHttpResponse.FromLogicResult(result);
    }
    
    [HttpPost]
    [Route("refresh")]
    public IActionResult OnRefresh([FromBody] RefreshTokenRequest request)
    {
        var result = _usersLogic.RefreshToken(request.RefreshToken);
        return GenericHttpResponse.FromLogicResult(result);
    }
    
    [HttpPost]
    [Route("logout")]
    public IActionResult OnLogout()
    {
        var result = _usersLogic.Logout();
        return GenericHttpResponse.FromLogicResult(result);
    }
}