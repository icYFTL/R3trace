using System.Data;
using AutoMapper;
using Computantis.database;
using Computantis.database.models;
using Computantis.extensions;
using Computantis.utils;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using R3TraceShared.database.stored_procedures;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Computantis.logic;

public class UsersLogic : BaseLogic
{
    private readonly ApplicationContext _db;
    private readonly IMapper _mapper;
    private readonly JwtUtils _jwtUtils;
    private readonly NationalitiesLogic _nationalitiesLogic;
    private readonly RequestInfo _requestInfo;
    private readonly ILogger<UsersLogic> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    public UsersLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _db = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _mapper = Scope.ServiceProvider.GetRequiredService<IMapper>();
        _jwtUtils = Scope.ServiceProvider.GetRequiredService<JwtUtils>();
        _nationalitiesLogic = Scope.ServiceProvider.GetRequiredService<NationalitiesLogic>();
        _requestInfo = Scope.ServiceProvider.GetRequiredService<RequestInfo>();
        _logger = Scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UsersLogic>();
        _configuration = Scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _scopeFactory = scopeFactory;
    }

    private GenericLogicResult _checkUser(User user)
    {
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

        // if (user.Username.Length < 3)
        // {
        //     return new FailedLogicResult
        //     {
        //         Result = "Too short username",
        //         StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
        //     };
        // }
        //
        // if (user.Password.Length < 7)
        // {
        //     return new FailedLogicResult
        //     {
        //         Result = "Too short password",
        //         StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
        //     };
        // }
        //
        if (_db.Users.Any(x => x.Username == user.Username))
        {
            return new FailedLogicResult
            {
                Result = "User with this username already exists",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (user.NationalityUid.HasValue)
        {
            if (!_nationalitiesLogic.IsNationalityValid(user.NationalityUid.Value).Status)
            {
                return new FailedLogicResult
                {
                    Result = "Not a valid nationality uid",
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
                };
            }
        }

        user.Uid = null;
        user.Salt = Utils.GenerateRandomString(32);
        user.Password += user.Salt;
        user.Password = user.Password.GetSha512();
        user.RegisteredIp = _requestInfo.RemoteIp;
        user.LastAccessIp = _requestInfo.RemoteIp;

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

    public GenericLogicResult UpdateUser(string? username, string? password, Guid? nationalityUid,
        Guid? teamUid)
    {
        var user = _db.Users.FirstOrDefault(x => x.Uid == _requestInfo.User!.Uid);
        if (user is null)
        {
            return new FailedLogicResult
            {
                Result = "User not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
            };
        }

        if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password) && !nationalityUid.HasValue &&
            !teamUid.HasValue)
        {
            return new FailedLogicResult
            {
                Result = "No any updates given",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (user.Password == (password + user.Salt).GetSha512())
        {
            return new FailedLogicResult
            {
                Result = "Previous password was the same",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (username == user.Username)
        {
            return new FailedLogicResult
            {
                Result = "Previous username was the same",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (!String.IsNullOrEmpty(username) && _db.Users.Any(x => x.Username == username))
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424),
                Result = "User with this username already exists"
            };
        }

        if (!String.IsNullOrEmpty(username))
        {
            user.Username = username;
        }

        if (!String.IsNullOrEmpty(password))
        {
            user.Password = password;
        }

        if (nationalityUid.HasValue)
        {
            user.NationalityUid = nationalityUid;
        }

        if (teamUid.HasValue)
        {
            user.TeamUid = teamUid;
        }

        var check = _checkUser(user);
        if (!check.Status)
            return check;

        if (!String.IsNullOrEmpty(password))
        {
            user.Salt = Utils.GenerateRandomString(32);
            user.Password += user.Salt;
            user.Password = user.Password.GetSha512();
        }

        if (nationalityUid.HasValue)
        {
            if (!_nationalitiesLogic.IsNationalityValid(user.NationalityUid!.Value).Status)
            {
                return new FailedLogicResult
                {
                    Result = "Not a valid nationality uid",
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
                };
            }
        }

        try
        {
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(500),
                Result = "Something went wrong",
                Exception = ex
            };
        }

        _requestInfo.Reload();
        return new SuccessLogicResult();
    }

    public GenericLogicResult DeleteUser()
    {
        using var connection = _db.GetNewConnection();
        var command = new NpgsqlCommand();

        var userUidParameter = new NpgsqlParameter("@userUid", NpgsqlDbType.Uuid);
        userUidParameter.Direction = ParameterDirection.Input;
        userUidParameter.Value = _requestInfo.User!.Uid!;
        command.Parameters.Add(userUidParameter);

        var softParameter = new NpgsqlParameter("@soft", NpgsqlDbType.Boolean);
        softParameter.Direction = ParameterDirection.Input;
        softParameter.Value = true;
        command.Parameters.Add(softParameter);

        var msgParameter = new NpgsqlParameter("@msg", NpgsqlDbType.Text);
        msgParameter.Direction = ParameterDirection.InputOutput;
        msgParameter.Value = "";
        command.Parameters.Add(msgParameter);

        var resultParameter = new NpgsqlParameter("@result", NpgsqlDbType.Boolean);
        resultParameter.Direction = ParameterDirection.InputOutput;
        resultParameter.Value = false;
        command.Parameters.Add(resultParameter);

        var deleteUserProcedure =
            new DeleteUserProcedure(_configuration.GetConnectionString("Postgres")!, _scopeFactory, ref command);

        var msg = String.Empty;
        var result = false;

        var spResult = deleteUserProcedure.RunNonQuery();
        if (spResult)
        {
            msg = (string)command.Parameters["@msg"].Value!;
            result = (bool)command.Parameters["@result"].Value!;
        }

        return new GenericLogicResult
        {
            Status = result!,
            Result = msg,
            StatusCode = result
                ? HttpUtils.HttpStatusCodeFromNumber(200)
                : HttpUtils.HttpStatusCodeFromNumber(424)
        };
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
            .Include(x => x.Nationality)
            .Include(x => x.Team)
            .Take(limit)
            .Skip(offset)
            .Where(x => !x.Deleted)
            .ToList()
            .Select(x => new
            {
                Uid = x.Uid,
                Username = x.Username,
                IsAdmin = x.IsAdmin,
                Trusted = x.Trusted,
                Banned = x.Banned,
                Nationality = x.Nationality,
                Team = x.Team is null
                    ? null
                    : new
                    {
                        Uid = x.Team.Uid,
                        Name = x.Team.Name,
                        Trusted = x.Team.Trusted,
                        Owner = x.Team.OwnerUid,
                        Users = x.Team.Users.Select(x => x.Uid)
                    }
            });

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetUser(Guid userUid)
    {
        var result = _db.Users
            .Include(x => x.Nationality)
            .Include(x => x.Team)
            // .Where(x => !x.Deleted)
            .Select(x => new
            {
                Uid = x.Uid,
                Username = x.Username,
                IsAdmin = x.IsAdmin,
                Trusted = x.Trusted,
                Banned = x.Banned,
                Nationality = x.Nationality,
                Team = x.Team != null
                    ? new
                    {
                        Uid = x.Team.Uid,
                        Name = x.Team.Name,
                        Trusted = x.Team.Trusted,
                        Owner = x.Team.OwnerUid,
                        Users = x.Team.Users.Select(u => u.Uid)
                    }
                    : null
            })
            .FirstOrDefault(x => x.Uid == userUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "User with this uid not found"
            };
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult SignIn(string username, string password)
    {
        var user = _db.Users.FirstOrDefault(x => x.Username == username);
        if (user is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(401),
                Result = "Invalid login or password"
            };
        }

        if (user.Password != (password + user.Salt).GetSha512())
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(401),
                Result = "Invalid login or password"
            };
        }

        if (user.Banned)
        {
            return new FailedLogicResult
            {
                Result = "Forbidden for you",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(403)
            };
        }

        if (user.Deleted)
        {
            return new FailedLogicResult
            {
                Result = "Account is inactive",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }


        user.LastAccessIp = _requestInfo.RemoteIp;
        user.LastAccessDate = DateTime.UtcNow;

        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(user.Uid!.Value, _requestInfo.RemoteIp);

        _db.RefreshTokens.Add(refreshToken);

        try
        {
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(500),
                Result = "Something went wrong",
                Exception = ex
            };
        }

        _requestInfo.Reload();

        return new SuccessLogicResult
        {
            Result = new
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            }
        };
    }

    public GenericLogicResult CheckToken(string token)
    {
        var result = _jwtUtils.ValidateJwtToken(token);

        if (result.HasValue)
        {
            var user = _db.Users.FirstOrDefault(x => x.Uid == result.Value);
            if (user is null) // KAWABANGA
            {
                return new FailedLogicResult
                {
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(401)
                };
            }

            if (user.Banned)
            {
                return new FailedLogicResult
                {
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(403)
                };
            }

            return new SuccessLogicResult
            {
                Result = new
                {
                    Uid = user.Uid,
                    IsAdmin = user.IsAdmin,
                    Banned = user.Banned,
                    Deleted = user.Deleted
                },
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(200)
            };
        }

        return new FailedLogicResult
        {
            StatusCode = HttpUtils.HttpStatusCodeFromNumber(401)
        };
    }

    public GenericLogicResult RefreshToken(string refreshToken)
    {
        var refresh = _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefault(x => x.Token == refreshToken);
        if (refresh is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "Refresh token not found"
            };
        }

        if (refresh.User.Banned)
        {
            return new FailedLogicResult
            {
                Result = "Forbidden for you",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(403)
            };
        }

        if (refresh.User.Deleted)
        {
            return new FailedLogicResult
            {
                Result = "Account is inactive",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }


        if (refresh.IsExpired)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424),
                Result = "Token expired"
            };
        }

        if (refresh.IsRevoked)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424),
                Result = "Token already used"
            };
        }

        var user = _db.Users
            .Include(x => x.Nationality)
            .Include(x => x.Team)
            .First(x => x.Uid == refresh.UserUid);

        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(user.Uid!.Value, _requestInfo.RemoteIp);

        refresh.RevokedAt = DateTime.UtcNow;
        refresh.RevokedIp = _requestInfo.RemoteIp;
        _db.RefreshTokens.Add(newRefreshToken);

        try
        {
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(500),
                Result = "Something went wrong",
                Exception = ex
            };
        }

        _requestInfo.Reload();

        return new SuccessLogicResult
        {
            Result = new
            {
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token
            }
        };
    }

    public GenericLogicResult GetMe()
    {
        return new SuccessLogicResult
        {
            Result = new
            {
                Uid = _requestInfo.User!.Uid,
                Username = _requestInfo.User!.Username,
                IsAdmin = _requestInfo.User!.IsAdmin,
                Trusted = _requestInfo.User!.Trusted,
                Banned = _requestInfo.User!.Banned,
                Nationality = _requestInfo.User!.Nationality,
                Team = _requestInfo.User!.Team
            }
        };
    }

    public GenericLogicResult Logout()
    {
        var refreshTokens = _requestInfo.User!.RefreshTokens;

        _db.AttachRange(refreshTokens);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedIp = _requestInfo.RemoteIp;
        }

        if (refreshTokens.Count > 10)
        {
            _db.RemoveRange(refreshTokens.Take(refreshTokens.Count - 10));
        }
        
        try
        {
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
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