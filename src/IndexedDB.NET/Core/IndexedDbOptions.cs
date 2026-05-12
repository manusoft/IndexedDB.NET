namespace ManuHub.IndexedDB.Core;

public sealed class IndexedDbOptions
{
    public string DatabaseName { get; set; } = "AppDb";
    public int Version { get; set; } = 1;
}