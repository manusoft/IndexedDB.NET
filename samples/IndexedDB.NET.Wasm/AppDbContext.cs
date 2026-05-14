using ManuHub.IndexedDB;
using ManuHub.IndexedDB.Context;
using ManuHub.IndexedDB.Queries;
using ManuHub.IndexedDB.Stores;

public class AppDbContext : IndexedDbContext
{
    public AppDbContext(ILogger<AppDbContext> logger)
        : base(new IndexedDbOptions
        {
            DatabaseName = "SampleDB",
            Version = 1
        },logger)
    {
    }

    // -----------------------------------------------------
    // ENTITY REGISTRY
    // -----------------------------------------------------

    protected override IEnumerable<Type> GetEntityTypes()
    {
        yield return typeof(TodoItem);
    }

    // -----------------------------------------------------
    // STORES
    // -----------------------------------------------------

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("todos");

    // OPTIONAL: Query access (clean API)
    public IndexedQuery<TodoItem> TodoQuery => Query<TodoItem>("todos");
}