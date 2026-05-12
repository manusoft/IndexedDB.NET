namespace ManuHub.IndexedDB.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IndexedPropertyAttribute : Attribute
{
    public bool Unique { get; set; }
}
