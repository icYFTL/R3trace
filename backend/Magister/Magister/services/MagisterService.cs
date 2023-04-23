using AutoMapper;
using Custos.services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Magister.database.models;
using Magister.logic;
using R3TraceShared.utils;

namespace Magister.services;

public class MagisterService : MagisterProtoService.MagisterProtoServiceBase
{
    private readonly TaskLogic _taskLogic;
    private readonly IMapper _mapper;

    public MagisterService(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _taskLogic = scope.ServiceProvider.GetRequiredService<TaskLogic>();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    public override Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, ServerCallContext context)
    {
        var result = _taskLogic.CreateTask(_mapper.Map<CtfTask>(request.Info));

        if (result.Status)
            return Task.FromResult(new CreateTaskResponse
            {
                Uid = result.Result!.ToString(),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new CreateTaskResponse
        {
            Message = result.Result!.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GenericResponse> DeleteTasks(DeleteTasksRequest request, ServerCallContext context)
    {
        var uids = new List<Guid>();
        foreach (var taskUid in request.Uids)
        {
            Guid.TryParse(taskUid, out var uid);
            if (uid == Guid.Empty)
            {
                return Task.FromResult(new GenericResponse
                {
                    Status = false,
                    Message = "Invalid uid",
                    StatusCode = 400
                });
            }

            uids.Add(uid);
        }


        var result = _taskLogic.DeleteTasks(uids);

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

    public override Task<GetTaskResponse> GetTask(GetTaskRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.Uid, out var uid);
        if (uid == Guid.Empty)
        {
            return Task.FromResult(new GetTaskResponse
            {
                StatusCode = 400
            });
        }

        var result = _taskLogic.GetTask(uid);
        if (result.Result is CtfTask)
            return Task.FromResult(new GetTaskResponse
            {
                Item = _mapper.Map<TaskProtoEntity>(result.Result),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GetTaskResponse
        {
            Item = null,
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GetTasksResponse> GetTasks(GetTasksRequest request, ServerCallContext context)
    {
        var tasks = _taskLogic.GetTasks(request.Offset, request.Limit);
        if (tasks.Result is List<CtfTask>)
        {
            var result = ((List<CtfTask>)tasks.Result!).Select(x => _mapper.Map<TaskProtoEntity>(x));

            return Task.FromResult(new GetTasksResponse
            {
                Items = { result }
            });
        }

        return Task.FromResult(new GetTasksResponse());
    }

    public override Task<GenericResponse> UpdateTasks(UpdateTasksRequest request, ServerCallContext context)
    {
        var tasks = request.Tasks.Select(x => _mapper.Map<CtfTask>(x)).ToList();
        var result = _taskLogic.UpdateTasks(tasks);

        return Task.FromResult(new GenericResponse
        {
            Status = result.Status,
            Message = result.Result?.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<CreateTaskTypeResponse> CreateTaskType(CreateTaskTypeRequest request,
        ServerCallContext context)
    {
        var result = _taskLogic.CreateTaskType(_mapper.Map<TaskType>(request.Info));

        if (result.Status)
            return Task.FromResult(new CreateTaskTypeResponse
            {
                Uid = result.Result!.ToString(),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new CreateTaskTypeResponse
        {
            Message = result.Result!.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GenericResponse> DeleteTaskTypes(DeleteTaskTypesRequest request, ServerCallContext context)
    {
        var uids = new List<Guid>();
        foreach (var taskUid in request.Uids)
        {
            Guid.TryParse(taskUid, out var uid);
            if (uid == Guid.Empty)
            {
                return Task.FromResult(new GenericResponse
                {
                    Status = false,
                    Message = "Invalid uid",
                    StatusCode = 400
                });
            }

            uids.Add(uid);
        }


        var result = _taskLogic.DeleteTaskTypes(uids);

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

    public override Task<GetTaskTypeResponse> GetTaskType(GetTaskTypeRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.Uid, out var uid);
        if (uid == Guid.Empty)
        {
            return Task.FromResult(new GetTaskTypeResponse
            {
                StatusCode = 400
            });
        }

        var result = _taskLogic.GetTaskType(uid);
        if (result.Result is TaskType)
            return Task.FromResult(new GetTaskTypeResponse
            {
                Item = _mapper.Map<TaskTypeProtoEntity>(result.Result),
                StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
            });

        return Task.FromResult(new GetTaskTypeResponse
        {
            Item = null,
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }

    public override Task<GetTaskTypesResponse> GetTaskTypes(Empty request, ServerCallContext context)
    {
        var result = ((List<TaskType>)_taskLogic.GetTaskTypes().Result!)
            .Select(x => _mapper.Map<TaskTypeProtoEntity>(x));

        return Task.FromResult(new GetTaskTypesResponse
        {
            Items = { result }
        });
    }

    public override Task<GenericResponse> UpdateTaskTypes(UpdateTasksRequest request, ServerCallContext context)
    {
        var taskTypes = request.Tasks.Select(x => _mapper.Map<TaskType>(x)).ToList();
        var result = _taskLogic.UpdateTaskTypes(taskTypes);

        return Task.FromResult(new GenericResponse
        {
            Status = result.Status,
            Message = result.Result?.ToString(),
            StatusCode = HttpUtils.NumberFromHttpStatusCode(result.StatusCode)
        });
    }
}