using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;

namespace Magister.database.models;

public class CtfTask : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Price { get; set; }
    public Guid TypeUid { get; set; }
    public bool Visible { get; set; }
    public Guid CtfUid { get; set; }

    [ForeignKey("TypeUid")] public TaskType TaskType { get; init; } = null!;
}