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
        var storeAttribute = type.GetCustomAttribute<IndexedStoreAttribute>()
            ?? throw new InvalidOperationException($"Missing [IndexedStoreAttribute] on {type.Name}");

        var definition = new StoreDefinition
        {
            Name = storeAttribute.Name
        };

        bool keyFound = false;

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetCustomAttribute<IndexedKeyAttribute>() is not null)
            {
                definition.KeyPath = property.Name;
                keyFound = true;

                // AutoIncrement only for int/long
                var pt = property.PropertyType;
                if (pt == typeof(int) || pt == typeof(long) || pt == typeof(uint) || pt == typeof(ulong))
                    definition.AutoIncrement = true;
            }

            var indexAttr = property.GetCustomAttribute<IndexedPropertyAttribute>();
            if (indexAttr is not null)
            {
                definition.Indexes.Add(new IndexDefinition
                {
                    Name = property.Name,
                    KeyPath = property.Name,
                    Unique = indexAttr.Unique
                });
            }
        }

        // FALLBACK - CRITICAL FOR Guid / Id
        if (!keyFound)
        {
            var idProp = type.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idProp != null)
            {
                definition.KeyPath = "Id";
                // Only enable AutoIncrement for numeric types
                var idType = idProp.PropertyType;
                definition.AutoIncrement = (idType == typeof(int) || idType == typeof(long));
            }
            else
            {
                // Last resort: use first public property
                var firstProp = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();
                if (firstProp != null)
                    definition.KeyPath = firstProp.Name;
            }
        }

        if (string.IsNullOrEmpty(definition.KeyPath))
            throw new InvalidOperationException($"Entity {type.Name} has no key defined. Add [IndexedKey] attribute.");

        return definition;
    }
}