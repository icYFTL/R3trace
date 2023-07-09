using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;
using R3TraceShared.database.models;

namespace Custos.database.models;

public class TeamInCtf : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public Guid TeamUid { get; set; }
    public Guid CtfUid { get; set; }

    [ForeignKey("CtfUid")] public virtual Ctf Ctf { get; init; }
}