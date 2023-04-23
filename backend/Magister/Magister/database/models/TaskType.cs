using R3TraceShared.database.interfaces;

namespace Magister.database.models;

public class TaskType : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}