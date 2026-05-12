namespace IndexedDB.NET.Core;

public sealed class IndexDefinition
{
    public string Name { get; set; } = string.Empty;
    public string KeyPath { get; set; } = string.Empty;
    public bool Unique { get; set; }
}