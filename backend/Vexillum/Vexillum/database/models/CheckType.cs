using R3TraceShared.database.interfaces;

namespace Vexillum.database.models;

public class CheckType : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; set; } = null!;
}