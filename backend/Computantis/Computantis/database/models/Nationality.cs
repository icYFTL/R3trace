using R3TraceShared.database.interfaces;

namespace Computantis.database.models;

public class Nationality : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; init; } = null!;
}