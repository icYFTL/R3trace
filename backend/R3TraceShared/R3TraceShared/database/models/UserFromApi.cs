namespace R3TraceShared.database.models;

public class UserFromApi
{
    public class NationalityFromApi
    {
        public Guid Uid { get; init; }
        public string Name { get; set; } = null!;
    }

    public class TeamFromApi
    {
        public Guid Uid { get; init; }
        public string Name { get; set; } = null!;
        public bool Trusted { get; init; }
        public Guid OwnerUid { get; init; }
        public List<Guid> Users { get; init; } = null!;
    }

    public Guid Uid { get; init; }
    public string Username { get; set; } = null!;
    public bool IsAdmin { get; init; }
    public bool Trusted { get; init; }
    public bool Banned { get; init; }
    public NationalityFromApi? Nationality { get; init; }
    public TeamFromApi? Team { get; init; }
}