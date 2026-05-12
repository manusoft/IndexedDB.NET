namespace ManuHub.IndexedDB.Abstractions;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IndexedPropertyAttribute : Attribute
{
    public bool Unique { get; set; }
}
