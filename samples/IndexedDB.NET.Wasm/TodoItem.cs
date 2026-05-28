using ManuHub.IndexedDB.Attributes;

[IndexedStore("Todos")]
public class TodoItem
{
    [IndexedKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = "";

    public bool IsDone { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}