using ManuHub.IndexedDB.Attributes;
using System.Reflection;

namespace ManuHub.IndexedDB.Metadata;

public sealed class StoreDefinition
{
    public string Name { get; set; } = string.Empty;

    public string KeyPath { get; set; } = string.Empty;

    public bool AutoIncrement { get; set; }

    public List<IndexDefinition> Indexes { get; set; } = [];

    public static StoreDefinition FromType(Type type)
    {
        var storeAttribute =
            type.GetCustomAttribute<IndexedStoreAttribute>()
            ?? throw new InvalidOperationException($"Missing IndexedStoreAttribute on {type.Name}");

        var definition = new StoreDefinition
        {
            Name = storeAttribute.Name
        };

        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<IndexedKeyAttribute>() is not null)
            {
                definition.KeyPath = property.Name;
            }

            var indexAttribute =
                property.GetCustomAttribute<IndexedPropertyAttribute>();

            if (indexAttribute is not null)
            {
                definition.Indexes.Add(new IndexDefinition
                {
                    Name = property.Name,
                    KeyPath = property.Name,
                    Unique = indexAttribute.Unique
                });
            }
        }

        return definition;
    }
}