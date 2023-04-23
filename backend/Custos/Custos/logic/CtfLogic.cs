using AutoMapper;
using Custos.database;
using Custos.database.models;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Custos.logic;

public class CtfLogic : BaseLogic
{
    private readonly ApplicationContext _db;
    private readonly IMapper _mapper;

    public CtfLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _db = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    private GenericLogicResult _ctfCheck(Ctf ctf)
    {
        if (ctf.EndDate < DateTime.UtcNow)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "End date can't be less than now"
            };
        }

        if (ctf.Name.Length > 256)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too long CTF name"
            };
        }

        if (ctf.Title.Length > 256)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too long CTF title"
            };
        }

        if (ctf.Code?.Length > 100)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too long CTF code"
            };
        }

        return new SuccessLogicResult();
    }

    public GenericLogicResult CreateCtf(Ctf ctf)
    {
        var checkResult = _ctfCheck(ctf);
        if (!checkResult.Status)
            return checkResult;

        ctf.Uid = null;

        try
        {
            _db.Ctfs.Add(ctf);
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
            Result = ctf.Uid
        };
    }

    public GenericLogicResult DeleteCtf(Guid ctfUid)
    {
        // TODO: Stored procedure
        return new FailedLogicResult();
    }

    public GenericLogicResult GetCtfs(int offset, int limit)
    {
        if (limit > 100)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too big limit. (100 max per request)"
            };
        }

        var result = _db.Ctfs
            .Take(limit)
            .Skip(offset)
            .ToList();

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetCtf(Guid ctfUid)
    {
        var result = _db.Ctfs.FirstOrDefault(x => x.Uid == ctfUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "Ctf with this uid not found"
            };
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult UpdateCtf(Ctf newCtf)
    {
        var ctf = _db.Ctfs.FirstOrDefault(x => x.Uid == newCtf.Uid);
        if (ctf is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "Ctf with this uid not found"
            };
        }

        var checkResult = _ctfCheck(newCtf);
        if (!checkResult.Status)
            return checkResult;

        try
        {
            // TODO: fix
            var updatedCtf = _mapper.Map<Ctf>(newCtf);
            var entityEntry = _db.Entry(ctf);
            if (entityEntry.State != EntityState.Detached)
            {
                entityEntry.State = EntityState.Detached;
            }

            _db.Update(updatedCtf);
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

        return new SuccessLogicResult();
    }
}