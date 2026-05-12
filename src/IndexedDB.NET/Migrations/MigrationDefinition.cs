namespace ManuHub.IndexedDB.Migrations;

public class MigrationDefinition
{
    public int Version { get; }
    public Action<MigrationBuilder> Action { get; }

    public MigrationDefinition(int version, Action<MigrationBuilder> action)
    {
        Version = version;
        Action = action;
    }
}