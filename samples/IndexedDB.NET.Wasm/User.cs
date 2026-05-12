using ManuHub.IndexedDB.Abstractions;

[IndexedStore("users")]
public sealed class User
{
    [IndexedKey]
    public Guid Id { get; set; }

    [IndexedProperty(Unique = true)]
    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}