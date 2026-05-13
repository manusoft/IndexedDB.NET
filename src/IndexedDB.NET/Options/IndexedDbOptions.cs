using ManuHub.IndexedDB.Migrations;

namespace ManuHub.IndexedDB;

public sealed class IndexedDbOptions
{
    public string DatabaseName { get; set; } = "AppDb";

    public int Version { get; set; } = 1;

    public MigrationCollection Migrations { get; } = new();
}