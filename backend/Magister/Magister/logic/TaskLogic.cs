using AutoMapper;
using Magister.database;
using Magister.database.models;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Magister.logic;

public partial class TaskLogic : BaseLogic
{
    private readonly ApplicationContext _db;
    private readonly IMapper _mapper;

    public TaskLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _db = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    private GenericLogicResult _taskCheck(CtfTask task)
    {
        if (task.Name.Length > 256)
        {
            return new FailedLogicResult
            {
                Result = "Too long name",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (task.Description.Length > 1024)
        {
            return new FailedLogicResult
            {
                Result = "Too long description",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (task.Price <= 0)
        {
            return new FailedLogicResult
            {
                Result = "Price must be > 0",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        var taskType = _db.TaskTypes.Any(x => x.Uid == task.TypeUid);
        if (!taskType)
        {
            return new FailedLogicResult
            {
                Result = "Not a valid type",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        return new SuccessLogicResult();
    }


    // TODO: Create tasks
    public GenericLogicResult CreateTask(CtfTask task)
    {
        var checkResult = _taskCheck(task);
        if (!checkResult.Status)
            return checkResult;

        task.Uid = null;

        try
        {
            _db.Tasks.Add(task);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(500),
                Result = "Something went wrong",
                Exception = ex
            };
        }

        return new SuccessLogicResult
        {
            Result = task.Uid
        };
    }

    public GenericLogicResult DeleteTasks(List<Guid> taskUids)
    {
        // TODO: Stored procedure
        return new FailedLogicResult();
    }

    public GenericLogicResult GetTasks(int offset, int limit)
    {
        if (limit > 100)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too big limit. (100 max per request)"
            };
        }

        var result = _db.Tasks
            .Include(x => x.TaskType)
            .Take(limit)
            .Skip(offset)
            .ToList();

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetTask(Guid taskUid)
    {
        var result = _db.Tasks
            .Include(x => x.TaskType)
            .FirstOrDefault(x => x.Uid == taskUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "Task with this uid not found"
            };
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult UpdateTasks(List<CtfTask> newCtfTasks)
    {
        foreach (var newTask in newCtfTasks)
        {
            var task = _db.Tasks.FirstOrDefault(x => x.Uid == newTask.Uid);
            if (task is null)
            {
                return new FailedLogicResult
                {
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                    Result = $"Task with uid '{newTask.Uid}' not found"
                };
            }

            var checkResult = _taskCheck(newTask);
            if (!checkResult.Status)
                return checkResult;

            try
            {
                // TODO: fix
                var updatedTask = _mapper.Map<Task>(newTask);
                var entityEntry = _db.Entry(task);
                if (entityEntry.State != EntityState.Detached)
                {
                    entityEntry.State = EntityState.Detached;
                }

                _db.Update(updatedTask);
            }
            catch (Exception ex)
            {
                return new FailedLogicResult
                {
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(500),
                    Result = "Something went wrong",
                    Exception = ex
                };
            }
        }

        _db.SaveChanges();

        return new SuccessLogicResult();
    }
}