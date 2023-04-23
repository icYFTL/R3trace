using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;

namespace Vexillum.database.models;

public class Flag : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Content { get; set; } = null!;
    public Guid CheckUid { get; set; }

    [ForeignKey("CheckUid")] public CheckType CheckType { get; init; } = null!;
}