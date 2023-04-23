using AutoMapper;
using Custos.database;
using Custos.database.models;
using Custos.logic;
using Grpc.Core;
using R3TraceShared.utils;

namespace Custos.services;

public class CustosService : CustosProtoService.CustosProtoServiceBase
{
    private readonly CtfLogic _ctfLogic;
    private readonly IMapper _mapper;

    public CustosService(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _ctfLogic = scope.ServiceProvider.GetRequiredService<CtfLogic>();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    public override Task<CreateCtfResponse> CreateCtf(CreateCtfRequest request, ServerCallContext context)
    {
        var result = _ctfLogic.CreateCtf(_mapper.Map<Ctf>(request.Info));

        if (result.Status)
            return Task.FromResult(new CreateCtfResponse
            {
                Uid = result.Result.ToString(),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new CreateCtfResponse
        {
            Message = result.Result!.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GenericResponse> DeleteCtf(DeleteCtfRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.Uid, out var uid);
        if (uid == Guid.Empty)
        {
            return Task.FromResult(new GenericResponse
            {
                Status = false,
                Message = "Invalid uid",
                StatusCode = 400
            });
        }

        var result = _ctfLogic.DeleteCtf(uid);

        if (result.Status)
            return Task.FromResult(new GenericResponse
            {
                Status = true,
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GenericResponse
        {
            Status = false,
            Message = result.Result?.ToString() ?? String.Empty,
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GetCtfResponse> GetCtf(GetCtfRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.Uid, out var uid);
        if (uid == Guid.Empty)
        {
            return Task.FromResult(new GetCtfResponse
            {
                StatusCode = 400
            });
        }

        var result = _ctfLogic.GetCtf(uid);
        if (result.Result is Ctf)
            return Task.FromResult(new GetCtfResponse
            {
                Item = _mapper.Map<CtfProtoEntity>(result.Result),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GetCtfResponse
        {
            Item = null,
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GetCtfsResponse> GetCtfs(GetCtfsRequest request, ServerCallContext context)
    {
        var result = ((List<Ctf>)_ctfLogic.GetCtfs(request.Offset, request.Limit).Result!)
            .Select(x => _mapper.Map<CtfProtoEntity>(x));

        return Task.FromResult(new GetCtfsResponse
        {
            Items = { result }
        });
    }

    public override Task<GenericResponse> UpdateCtf(UpdateCtfRequest request, ServerCallContext context)
    {
        var result = _ctfLogic.UpdateCtf(_mapper.Map<Ctf>(request.Info));

        return Task.FromResult(new GenericResponse
        {
            Status = result.Status,
            Message = result.Result?.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }
}