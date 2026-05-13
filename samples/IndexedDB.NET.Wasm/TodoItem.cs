using ManuHub.IndexedDB.Attributes;

[IndexedStore("todos")]
public class TodoItem
{
    [IndexedKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = "";

    public bool IsDone { get; set; }
}