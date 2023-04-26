using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using R3TraceShared.database.interfaces;

namespace Computantis.database.models;

public class User : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Username { get; set; } = null!;
    [IgnoreDataMember]
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public Guid? NationalityUid { get; set; }
    public Guid? TeamUid { get; set; }

    [ForeignKey("NationalityUid")] public virtual Nationality Nationality { get; init; } = null!;
    [ForeignKey("TeamUid")] public virtual Team? Team { get; init; }
}