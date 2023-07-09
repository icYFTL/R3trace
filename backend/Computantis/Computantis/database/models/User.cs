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
    [IgnoreDataMember]
    public string Salt { get; set; } = null!;
    [IgnoreDataMember]
    public Guid? NationalityUid { get; set; }
    [IgnoreDataMember]
    public Guid? TeamUid { get; set; }
    public bool IsAdmin { get; set; }
    public bool Trusted { get; set; }
    public bool Banned { get; set; }
    public bool Deleted { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastAccessDate { get; set; }
    public string RegisteredIp { get; set; } = null!;
    public string LastAccessIp { get; set; } = null!;
    
    [ForeignKey("NationalityUid")] public virtual Nationality Nationality { get; set; } = null!;
    [ForeignKey("TeamUid")] public virtual Team? Team { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = null!;
}