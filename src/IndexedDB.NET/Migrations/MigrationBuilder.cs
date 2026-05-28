using ManuHub.IndexedDB.Metadata;

namespace ManuHub.IndexedDB.Migrations;

public sealed class MigrationBuilder
{
    internal List<StoreDefinition> Stores { get; } = [];
    internal Dictionary<string, Action<object>> DataTransformers { get; } = new();

    public void CreateStore<T>()
    {
        Stores.Add(StoreDefinition.FromType(typeof(T)));
    }

    /// <summary>
    /// Add custom data transformation logic during migration
    /// </summary>
    public void TransformData<T>(Func<T, T> transformer)
    {
        DataTransformers[typeof(T).Name] = entity => transformer((T)entity);
    }
}