using ManuHub.IndexedDB.Migrations;

namespace ManuHub.IndexedDB;

public sealed class IndexedDbOptions
{
    public string DatabaseName { get; set; } = "MyAppDb";
    public int Version { get; set; } = 10;   // Keep stable

    public MigrationCollection Migrations { get; } = new();

    internal int GetEffectiveVersion()
    {
        var migrations = Migrations.GetAll();
        if (migrations.Count == 0)
            return Version;

        var maxMigration = migrations.Max(m => m.Version);
        return Math.Max(Version, maxMigration);
    }
}