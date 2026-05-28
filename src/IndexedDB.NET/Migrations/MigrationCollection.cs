namespace ManuHub.IndexedDB.Migrations;

public sealed class MigrationCollection
{
    private readonly List<MigrationDefinition> _migrations = [];

    public void Add(int version, Action<MigrationBuilder> configure)
    {
        if (version < 1)
            throw new ArgumentException("Version must be >= 1", nameof(version));

        _migrations.Add(new MigrationDefinition(version, configure));
    }

    public IReadOnlyList<MigrationDefinition> GetAll()
    {
        return _migrations
            .OrderBy(x => x.Version)
            .ToList();
    }
}