using ManuHub.IndexedDB.Metadata;

namespace ManuHub.IndexedDB.Migrations;

public sealed class MigrationBuilder
{
    internal List<StoreDefinition> Stores { get; } = [];

    public void CreateStore<T>()
    {
        Stores.Add(StoreDefinition.FromType(typeof(T)));
    }
}