using IndexedDB.NET.Wasm;
using ManuHub.IndexedDB;
using ManuHub.IndexedDB.Context;
using ManuHub.IndexedDB.Stores;

public class AppDbContext : IndexedDbContext
{
    public AppDbContext(IndexedDbOptions options) : base(options) { }

    protected override IEnumerable<Type> GetEntityTypes() =>
        [typeof(TodoItem), typeof(Employee)];

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("Todos");
    public IndexedSet<Employee> Employees => Set<Employee>("Employees");
}