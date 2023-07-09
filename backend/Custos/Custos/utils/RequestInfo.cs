using R3TraceShared.communicators;
using R3TraceShared.database.models;
using R3TraceShared.http;

namespace Custos.utils;

public sealed class RequestInfo : RequestInfoBase
{
    private ILogger<RequestInfo> _logger;
    private UserFromApi? _user;
    private readonly ComputantisCommunicator _computantisCommunicator;

    public UserFromApi? User
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

            _user = _computantisCommunicator.GetUser(userUid).Result;

            return _user;
        }
    }

    public RequestInfo(IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory) : base(contextAccessor,
        scopeFactory)
    {
        _computantisCommunicator = Scope.ServiceProvider.GetRequiredService<ComputantisCommunicator>();
    }

    protected override void Init()
    {
        RemoteIp = HttpContext!.Connection.RemoteIpAddress!.ToString();
    }

    public override void Reload()
    {
        _user = null;
        Init();
    }
}