using System.ComponentModel.DataAnnotations.Schema;
using R3TraceShared.database.interfaces;

namespace Computantis.database.models;

public class RefreshToken : IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Token { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedIp { get; init; } = null!;
    public DateTime? RevokedAt { get; set; }

    public string? RevokedIp { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    public Guid UserUid { get; init; }
    [ForeignKey("UserUid")] public User User { get; init; } = null!;
}