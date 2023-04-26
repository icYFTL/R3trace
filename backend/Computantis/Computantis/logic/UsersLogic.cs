using AutoMapper;
using Computantis.database;
using Computantis.database.models;
using Computantis.extensions;
using Computantis.utils;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Computantis.logic;

public class UsersLogic : BaseLogic
{
    private readonly ApplicationContext _db;
    private readonly IMapper _mapper;
    
    public UsersLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    }

    private GenericLogicResult _checkUser(User user)
    {
        if (_db.Users.Any(x => x.Username == user.Username))
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "User with this username already exists"
            };
        }
        if (user.Username.Length > 32)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too long username"
            };
        }

        if (user.Username.Length < 3)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too short username"
            };
        }

        if (user.Password.Length > 128)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too long password"
            };
        }
        
        if (user.Password.Length < 8 && user.Password.Length != 128 && user.Password != String.Empty)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too short password"
            };
        }

        return new SuccessLogicResult();
    }

    public GenericLogicResult CreateUser(User user)
    {
        var check = _checkUser(user);
        if (!check.Status)
            return check;

        user.Uid = null;
        user.Salt = Utils.GenerateRandomString(32);
        user.Password += user.Salt;
        user.Password = user.Password.GetSha512();

        try
        {
            _db.Users.Add(user);
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

    public GenericLogicResult UpdateUser(User newUser)
    {
        var user = _db.Users.FirstOrDefault(x => x.Uid == newUser.Uid);
        if (user is null)
        {
            return new FailedLogicResult
            {
                Result = "User not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
            };
        }
        
        var check = _checkUser(newUser);
        if (!check.Status)
            return check;
        
        try
        {
            // TODO: fix
            var updatedUser = _mapper.Map<User>(newUser);
            var entityEntry = _db.Entry(newUser);
            if (entityEntry.State != EntityState.Detached)
            {
                entityEntry.State = EntityState.Detached;
            }

            _db.Update(updatedUser);
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

    public GenericLogicResult DeleteUser(Guid userUid)
    {
        // TODO: Stored procedure
        return new FailedLogicResult();
    }

    public GenericLogicResult GetUsers(int limit, int offset)
    {
        if (limit > 100)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too big limit. (100 max per request)"
            };
        }
        
        var result = _db.Users
            .Take(limit)
            .Skip(offset)
            .ToList();

        foreach (var user in result)
        {
            user.Password = String.Empty;
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }
    
    public GenericLogicResult GetUser(Guid userUid)
    {
        var result = _db.Users.FirstOrDefault(x => x.Uid == userUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "User with this uid not found"
            };
        }

        result.Password = String.Empty;

        return new SuccessLogicResult
        {
            Result = result
        };
    }
}