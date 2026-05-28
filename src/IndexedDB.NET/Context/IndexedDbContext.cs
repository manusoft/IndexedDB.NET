using ManuHub.IndexedDB.Metadata;
using ManuHub.IndexedDB.Migrations;
using ManuHub.IndexedDB.Queries;
using ManuHub.IndexedDB.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace ManuHub.IndexedDB.Context;

public abstract class IndexedDbContext
{
    private readonly IndexedDbOptions _options;
    private IJSRuntime? _js;
    private IJSObjectReference? _module;
    private bool _initialized;
    private readonly SemaphoreSlim _lock = new(1, 1);

    internal IServiceProvider Services { get; set; } = default!;

    protected IndexedDbContext(IndexedDbOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected abstract IEnumerable<Type> GetEntityTypes();

    private IJSRuntime JS => _js ??= Services.GetRequiredService<IJSRuntime>();

    private async Task EnsureReadyAsync()
    {
        if (_initialized) return;

        await _lock.WaitAsync();
        try
        {
            if (_initialized) return;

            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ManuHub.IndexedDB/indexeddb.js");

            var currentStores = GetEntityTypes()
                .Select(StoreDefinition.FromType)
                .ToList();

            await ApplyMigrationsAsync(currentStores);

            _initialized = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task ApplyMigrationsAsync(List<StoreDefinition> currentStores)
    {
        var effectiveVersion = _options.GetEffectiveVersion();

        try
        {
            var migrations = _options.Migrations.GetAll();

            foreach (var migration in migrations)
            {
                var builder = new MigrationBuilder();
                migration.Configure(builder);
                await _module!.InvokeVoidAsync("applyMigration",
                    _options.DatabaseName, migration.Version, builder.Stores);
            }

            await _module!.InvokeVoidAsync("initializeDatabase",
                _options.DatabaseName, effectiveVersion, currentStores);

            Console.WriteLine($"[IndexedDB] ✅ Database ready (v{effectiveVersion})");
        }
        catch (JSException ex)
        {
            if (ex.Message.Contains("less than the existing version"))
            {
                // Silent handling - use existing database (most common case)
                await _module!.InvokeVoidAsync("initializeDatabase",
                    _options.DatabaseName, effectiveVersion, currentStores);

                Console.WriteLine($"[IndexedDB] ✅ Using existing database");
            }
            else
            {
                Console.Error.WriteLine($"[IndexedDB] Error: {ex.Message}");
                throw;
            }
        }
    }

    internal async Task<IJSObjectReference> Module()
    {
        await EnsureReadyAsync();
        return _module!;
    }

    protected IndexedSet<T> Set<T>(string storeName) => new(Module, _options.DatabaseName, storeName);
    protected IndexedQuery<T> Query<T>(string storeName) => new(Module, _options.DatabaseName, storeName);
}