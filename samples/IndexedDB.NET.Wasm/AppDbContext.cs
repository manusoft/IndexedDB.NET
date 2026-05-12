using ManuHub.IndexedDB.Core;
using Microsoft.JSInterop;

public sealed class AppDbContext : IndexedDbContext
{
    public AppDbContext(
        IJSRuntime jsRuntime,
        IndexedDbOptions options)
        : base(jsRuntime, options)
    {
    }

    public IndexedSet<User> Users => Set<User>("users");

    protected override List<StoreDefinition> GetStores()
    {
        return
        [
            new StoreDefinition
            {
                Name = "users",
                KeyPath = "id",
                AutoIncrement = false,

                Indexes =
                [
                    new IndexDefinition
                    {
                        Name = "email",
                        KeyPath = "email",
                        Unique = true
                    }
                ]
            }
        ];
    }
}