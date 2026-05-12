namespace ManuHub.IndexedDB.Core;

public sealed class StoreDefinition
{
    public string Name { get; set; } = string.Empty;
    public string KeyPath { get; set; } = "id";
    public bool AutoIncrement { get; set; }
    public List<IndexDefinition> Indexes { get; set; } = [];
}
