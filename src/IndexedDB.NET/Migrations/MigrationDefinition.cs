namespace ManuHub.IndexedDB.Migrations;

public class MigrationDefinition
{
    public int Version { get; }
    public Action<MigrationBuilder> Configure { get; }

    public MigrationDefinition(int version, Action<MigrationBuilder> configure)
    {
        Version = version;
        Configure = configure;
    }
}