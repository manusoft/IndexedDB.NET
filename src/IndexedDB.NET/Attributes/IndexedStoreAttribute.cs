namespace ManuHub.IndexedDB.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class IndexedStoreAttribute : Attribute
{
    public string Name { get; }

    public IndexedStoreAttribute(string name)
    {
        Name = name;
    }
}