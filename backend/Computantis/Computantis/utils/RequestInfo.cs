using Computantis.database;
using Computantis.database.models;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.http;

namespace Computantis.utils;

public sealed class RequestInfo : RequestInfoBase
{
    private ApplicationContext _db;
    private ILogger<RequestInfo> _logger;
    private User? _user;

    public User? User
    {
        get
        {
            if (_user is not null)
                return _user;

            // _logger = Scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<RequestInfo>();
            // _logger.LogError(String.Join(" ", HttpContext.Request.Headers.Select(x => $"{x.Key}:{x.Value}")));
            
            if (!HttpContext.Request.Headers.TryGetValue("Identify", out var identify) ||
                !Guid.TryParse(identify.FirstOrDefault(), out var userUid) ||
                userUid == Guid.Empty)
            {
                return null;
            }

            _user = _db.Users
                .Include(x => x.Nationality)
                .Include(x => x.Team)
                .Include(x => x.RefreshTokens)
                .FirstOrDefault(x => x.Uid == userUid);

            return _user;
        }
    }

    public RequestInfo(IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory) : base(contextAccessor,
        scopeFactory)
    {
    }

    protected override void Init()
    {
        RemoteIp = HttpContext!.Connection.RemoteIpAddress!.ToString();
        _db = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    }

    public override void Reload()
    {
        _user = null;
        Init();
    }
}