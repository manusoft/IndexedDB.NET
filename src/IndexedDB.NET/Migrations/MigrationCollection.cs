namespace ManuHub.IndexedDB.Migrations;

public sealed class MigrationCollection
{
    private readonly List<MigrationDefinition> _migrations = [];

    public void Add(int version, Action<MigrationBuilder> action)
    {
        _migrations.Add(new MigrationDefinition(version, action));
    }

    public IReadOnlyList<MigrationDefinition> GetAll()
    {
        return _migrations.OrderBy(x => x.Version).ToList();
    }
}