using ManuHub.IndexedDB.Attributes;

namespace IndexedDB.NET.Wasm;

[IndexedStore("Employees")]
public class Employee
{
    [IndexedKey]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
}

public enum Gender { Male, Female }