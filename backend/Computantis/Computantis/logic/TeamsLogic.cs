using Computantis.database;
using Computantis.database.models;
using Computantis.utils;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.extensions;
using R3TraceShared.logic;
using R3TraceShared.utils;

namespace Computantis.logic;

public class TeamsLogic : BaseLogic
{
    private readonly ApplicationContext _db;
    private readonly List<WordsBlackList> _badWords;
    private readonly RequestInfo _requestInfo;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TeamsLogic> _logger;

    public TeamsLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        _db = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _badWords = _db.Cache(x => x.WordsBlackList.ToList(), "words_black_list", TimeSpan.FromHours(1));
        _requestInfo = Scope.ServiceProvider.GetRequiredService<RequestInfo>();
        _configuration = Scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _logger = Scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<TeamsLogic>();
    }

    private GenericLogicResult _checkTeam(Team team)
    {
        if (team.Name.Length < 5)
        {
            return new FailedLogicResult
            {
                Result = "Too short team name",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        foreach (var word in _badWords)
        {
            if (GeneralUtils.TextSimilarity(word.Word, team.Name) > 0.6)
            {
                team.Trusted = false;
            }
        }

        // var ownerUid = team.OwnerUid;
        // if (!_db.Users.Any(x => x.Uid == ownerUid))
        // {
        //     return new FailedLogicResult
        //     {
        //         Result = "Owner not found",
        //         StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
        //     };
        // }

        return new SuccessLogicResult
        {
            Result = team
        };
    }

    public GenericLogicResult CreateTeam(Team team)
    {
        // TODO: FIX
        _db.Attach(_requestInfo.User!);
        var check = _checkTeam(team);
        if (!check.Status)
            return check;

        team = (Team)check.Result!;

        if (_db.Teams.Any(x => x.Name == team.Name))
        {
            return new FailedLogicResult
            {
                Result = "Team with this name already exists",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (_requestInfo.User!.Team is not null)
        {
            return new FailedLogicResult
            {
                Result = "You're already in a team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        team.Code = GeneralUtils.RandomString(64);
        team.Uid = Guid.NewGuid();
        team.OwnerUid = _requestInfo.User!.Uid!.Value;

        _requestInfo.User!.TeamUid = team.Uid;

        try
        {
            _db.Teams.Add(team);
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


        try
        {
            _db.Teams.Add(team);
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

    public GenericLogicResult UpdateTeam(string? name, bool? code, Guid? ownerUid)
    {
        if (_requestInfo.User!.Team is null)
        {
            return new FailedLogicResult
            {
                Result = "You are not in a team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (_requestInfo.User!.Team.Deleted)
        {
            return new FailedLogicResult
            {
                Result = "Team is inactive",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (_requestInfo.User!.Team.OwnerUid != _requestInfo.User.Uid)
        {
            return new FailedLogicResult
            {
                Result = "You don't have permission",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(403)
            };
        }

        _db.Attach(_requestInfo.User.Team);


        if (!String.IsNullOrEmpty(name))
        {
            _requestInfo.User!.Team.Name = name;
        }

        if (code.HasValue)
        {
            _requestInfo.User!.Team.Code = GeneralUtils.RandomString(64);
        }

        if (ownerUid.HasValue)
        {
            var newOwner = _db.Users.FirstOrDefault(x => x.Uid == ownerUid.Value);
            if (newOwner is null)
            {
                return new FailedLogicResult
                {
                    Result = "New owner not found",
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
                };
            }

            if (newOwner.TeamUid != _requestInfo.User!.Team.Uid)
            {
                return new FailedLogicResult
                {
                    Result = "New owner must be in the current team",
                    StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
                };
            }

            _requestInfo.User!.Team.OwnerUid = ownerUid.Value;
        }

        var check = _checkTeam(_requestInfo.User!.Team);
        if (!check.Status)
            return check;

        _requestInfo.User!.Team = (Team)check.Result!;

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

    public GenericLogicResult DeleteTeam(Guid teamUid)
    {
        // TODO: Stored procedure
        return new FailedLogicResult();
    }

    public GenericLogicResult GetTeams(int limit, int offset)
    {
        if (limit > 100)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400),
                Result = "Too big limit. (100 max per request)"
            };
        }

        var result = _db.Teams
            .Include(x => x.Users)
            .Take(limit)
            .Skip(offset)
            .Where(x => !x.Deleted)
            .ToList()
            .Select(x => new
            {
                Uid = x.Uid,
                Name = x.Name,
                Trusted = x.Trusted,
                CreateDate = x.CreateDate,
                OwnerUid = x.OwnerUid,
                Users = x.Users.Select(x => x.Uid).ToList()
            });

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetTeam(Guid teamUid)
    {
        var result = _db.Teams
            .Include(x => x.Owner)
            .Include(x => x.Users)
            // .Where(x => !x.Deleted)
            .Select(x => new
            {
                Uid = x.Uid,
                Name = x.Name,
                Trusted = x.Trusted,
                CreateDate = x.CreateDate,
                OwnerUid = x.OwnerUid,
                Users = x.Users.Select(x => x.Uid).ToList()
            })
            .FirstOrDefault(x => x.Uid == teamUid);
        if (result is null)
        {
            return new FailedLogicResult
            {
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404),
                Result = "Team with this uid not found"
            };
        }

        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult GetMyTeam()
    {
        var exists = _requestInfo.User!.Team is not null;
        if (exists)
        {
            return new SuccessLogicResult
            {
                Result = new
                {
                    Uid = _requestInfo.User!.Team!.Uid,
                    Name = _requestInfo.User!.Team!.Name,
                    Code = _requestInfo.User!.Team!.Code,
                    OwnerUid = _requestInfo.User!.Team!.OwnerUid,
                    Trusted = _requestInfo.User!.Team!.Trusted,
                    Deleted = _requestInfo.User!.Team!.Deleted,
                    CreateDate = _requestInfo.User!.Team!.CreateDate,
                    Users = _requestInfo.User!.Team!.Users.Select(x => new
                    {
                        Uid = x.Uid,
                        Username = x.Username,
                        Trusted = x.Trusted,
                        Banned = x.Banned,
                        Deleted = x.Deleted
                    })
                }
            };
        }

        return new FailedLogicResult
        {
            StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
        };
    }

    public GenericLogicResult JoinTeam(string code)
    {
        var team = _db.Teams
            .Include(x => x.Users)
            .FirstOrDefault(x => x.Code == code);

        if (team is null)
        {
            return new FailedLogicResult
            {
                Result = "Team not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
            };
        }

        if (team.Users.Select(x => x.Uid).Contains(_requestInfo.User!.Uid!))
        {
            return new FailedLogicResult
            {
                Result = "You're already in a team. Leave from current first",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (team.Users.Count >= int.Parse(_configuration["GeneralSettings:MaxTeamMembers"]!))
        {
            return new FailedLogicResult
            {
                Result = "Too many members in the team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        _db.Attach(_requestInfo.User!);
        _requestInfo.User.TeamUid = team.Uid;

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

    public GenericLogicResult LeaveTeam()
    {
        if (_requestInfo.User!.TeamUid is null)
        {
            return new FailedLogicResult
            {
                Result = "You're not in a team yet",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (_requestInfo.User!.Team!.OwnerUid == _requestInfo.User!.Uid!)
        {
            return new FailedLogicResult
            {
                Result = "You can't leave from a team where you're owner. Delegate the team first.",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        _db.Attach(_requestInfo.User!);
        _requestInfo.User.TeamUid = null;

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

    public GenericLogicResult DelegateTeam(Guid newOwnerUid)
    {
        var newOwner = _db.Users.FirstOrDefault(x => x.Uid == newOwnerUid);
        if (newOwner == null)
        {
            return new FailedLogicResult
            {
                Result = "New owner not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(404)
            };
        }

        if (_requestInfo.User!.TeamUid is null)
        {
            return new FailedLogicResult
            {
                Result = "You're not in a team yet",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (_requestInfo.User!.Team!.OwnerUid != _requestInfo.User!.Uid)
        {
            return new FailedLogicResult
            {
                Result = "You're not an owner of current team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (newOwner.TeamUid != _requestInfo.User!.TeamUid)
        {
            return new FailedLogicResult
            {
                Result = "New owner must be in your team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        _db.Attach(_requestInfo.User!);
        _requestInfo.User!.Team!.OwnerUid = newOwnerUid;

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

    public GenericLogicResult AddUserToTeam(Guid userUid, Guid teamUid)
    {
        var user = _db.Users.FirstOrDefault(x => x.Uid == userUid);
        if (user is null)
        {
            return new FailedLogicResult
            {
                Result = "User not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        var team = _db.Teams
            .Include(x => x.Users)
            .FirstOrDefault(x => x.Uid == teamUid);
        if (team is null)
        {
            return new FailedLogicResult
            {
                Result = "Team not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (team.Users.Select(x => x.Uid).Contains(userUid))
        {
            return new FailedLogicResult
            {
                Result = "User already in team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        if (team.Users.Count >= int.Parse(_configuration["GeneralSettings:MaxTeamMembers"]!))
        {
            return new FailedLogicResult
            {
                Result = "Too many members in team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        user.TeamUid = teamUid;

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

    public GenericLogicResult RemoveUserFromTeam(Guid userUid)
    {
        var user = _db.Users.FirstOrDefault(x => x.Uid == userUid);
        if (user is null)
        {
            return new FailedLogicResult
            {
                Result = "User not found",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(400)
            };
        }

        if (user.TeamUid is null)
        {
            return new FailedLogicResult
            {
                Result = "User not in a team",
                StatusCode = HttpUtils.HttpStatusCodeFromNumber(424)
            };
        }

        user.TeamUid = null;

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