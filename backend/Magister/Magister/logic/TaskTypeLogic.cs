using Magister.database.models;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Magister.logic;

public partial class TaskLogic : BaseLogic
{
    private GenericLogicResult _typeCheck(TaskType type)
    {
        if (type.Name.Length > 256)
        {
            return new FailedLogicResult
            {
                Result = "Too long name",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (type.Description?.Length > 1024)
        {
            return new FailedLogicResult
            {
                Result = "Too long description",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }
        
        var taskType = _db.TaskTypes.Any(x => x.Name == type.Name);
        if (taskType)
        {
            return new FailedLogicResult
            {
                Result = "TaskType with this name already exists",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        return new SuccessLogicResult();
    }
    
    public GenericLogicResult CreateTaskType(TaskType type)
    {
        var checkResult = _typeCheck(type);
        if (!checkResult.Status)
            return checkResult;

        type.Uid = null;

        try
        {
            _db.TaskTypes.Add(type);
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
            Result = type.Uid
        };
    }

    public GenericLogicResult DeleteTaskTypes(List<Guid> typesUids)
    {
        // TODO: Stored procedure
        return new FailedLogicResult();
    }

    public GenericLogicResult GetTaskTypes()
    {
        var result = _db.TaskTypes.ToList();

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetTaskType(Guid typeUid)
    {
        var result = _db.TaskTypes.FirstOrDefault(x => x.Uid == typeUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "TaskType with this uid not found"
            };
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult UpdateTaskTypes(List<TaskType> newTaskTypes)
    {
        foreach (var newTaskType in newTaskTypes)
        {
            var taskType = _db.TaskTypes.FirstOrDefault(x => x.Uid == newTaskType.Uid);
            if (taskType is null)
            {
                return new FailedLogicResult
                {
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                    Result = $"TaskType with uid '{newTaskType.Uid}' not found"
                };
            }

            var checkResult = _typeCheck(newTaskType);
            if (!checkResult.Status)
                return checkResult;

            try
            {
                // TODO: fix
                var updatedTask = _mapper.Map<TaskType>(newTaskType);
                var entityEntry = _db.Entry(taskType);
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