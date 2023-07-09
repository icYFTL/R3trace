using R3TraceShared.database.interfaces;

namespace Computantis.database.models;

public class WordsBlackList: IBaseDatabaseEntity
{
    public Guid? Uid { get; set; }
    public string Word { get; set; } = null!;
    public string? Description { get; set; }
}