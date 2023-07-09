using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;

namespace Computantis.database.models;

public class Team : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public Guid OwnerUid { get; set; }
    public bool Trusted { get; set; }
    public bool Deleted { get; set; }
    public DateTime CreateDate { get; set; }
    [ForeignKey("OwnerUid")] public virtual User Owner { get; init; } = null!;
    public virtual IList<User> Users { get; init; } = null!;
}