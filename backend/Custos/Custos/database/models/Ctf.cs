using Newtonsoft.Json;
using R3TraceShared.database.interfaces;
using R3TraceShared.database.models;

namespace Custos.database.models;

public class Ctf : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [JsonIgnore]
    public string? Code { get; set; }

    public virtual IList<Setting>? Settings { get; init; }
    // public virtual IList<User>? Users { get; init; }
}