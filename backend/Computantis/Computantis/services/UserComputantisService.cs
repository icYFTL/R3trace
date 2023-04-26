using AutoMapper;
using Computantis.database.models;
using Computantis.logic;
using Grpc.Core;
using R3TraceShared.utils;

namespace Computantis.services;

public partial class ComputantisService : ComputantisProtoService.ComputantisProtoServiceBase
{
    private readonly UsersLogic _usersLogic;
    private readonly NationalitiesLogic _nationalitiesLogic;
    private readonly IMapper _mapper;

    public ComputantisService(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _usersLogic = scope.ServiceProvider.GetRequiredService<UsersLogic>();
        _nationalitiesLogic = scope.ServiceProvider.GetRequiredService<NationalitiesLogic>();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    public override Task<GenericResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var result = _usersLogic.CreateUser(_mapper.Map<User>(request.User));

        if (result.Status)
            return Task.FromResult(new GenericResponse
            {
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GenericResponse
        {
            Message = result.Result!.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GenericResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var result = _usersLogic.UpdateUser(_mapper.Map<User>(request.User));

        return Task.FromResult(new GenericResponse
        {
            Status = result.Status,
            Message = result.Result?.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
    {
        var result = ((List<User>)_usersLogic.GetUsers(request.Limit, request.Offset).Result!)
            .Select(x => _mapper.Map<UserProtoEntity>(x));

        return Task.FromResult(new GetUsersResponse
        {
            Users = { result }
        });
    }

    public override Task<GenericResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
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

        var result = _usersLogic.DeleteUser(uid);

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

    public override Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.Uid, out var uid);
        if (uid == Guid.Empty)
        {
            return Task.FromResult(new GetUserResponse
            {
                StatusCode = 400
            });
        }

        var result = _usersLogic.GetUser(uid);
        if (result.Result is User)
            return Task.FromResult(new GetUserResponse
            {
                User = _mapper.Map<UserProtoEntity>(result.Result),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GetUserResponse
        {
            User = null,
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }
}