using Computantis.database;
using R3TraceShared.extensions;
using R3TraceShared.logic;

namespace Computantis.logic;

public class NationalitiesLogic : BaseLogic
{
    private readonly ApplicationContext _db;

    public NationalitiesLogic(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    }

    public GenericLogicResult GetNationalities()
    {
        var result = _db.Nationalities
            .Cache(x => x.ToList(), "nationalities_list", TimeSpan.FromDays(1), false);
        return new SuccessLogicResult
        {
            Result = result
        };
    }

    public GenericLogicResult IsNationalityValid(Guid nationalityUid)
    {
        var result = _db.Nationalities
            .Any(x => x.Uid == nationalityUid);
        return new GenericLogicResult
        {
            Status = result
        };
    }
}