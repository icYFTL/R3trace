using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;

namespace Custos.database.models;

public class Setting : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public Guid CtfUid { get; set; }

    [ForeignKey("CtfUid")] public virtual Ctf Ctf { get; init; } = null!;
}