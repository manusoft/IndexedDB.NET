﻿![Static Badge](https://img.shields.io/badge/ManuHub.IndexedDB-blue) 
![NuGet Version](https://img.shields.io/nuget/v/ManuHub.IndexedDB) 
![NuGet Downloads](https://img.shields.io/nuget/dt/ManuHub.IndexedDB)

# 📦 IndexedDB.NET

> A lightweight, production-ready **IndexedDB wrapper for Blazor WebAssembly** with EF-Core-inspired API.

---

## 🚀 Overview

**ManuHub.IndexedDB is a robust, multi-entity IndexedDB solution for Blazor WebAssembly. It provides a clean, familiar .NET API while handling the complexities of browser storage.

---

## ♻️ Changelog v1.2

### Major Improvements
- **Full Multi-Entity Support** 
- **Robust Migration System** with version management
- **Stable Data Persistence** 
- **Smart Entity Normalization** — Works with `Guid`, `int`, `string` keys
- **Improved Version Conflict Handling**
- **Better Error Messages & Debugging**
- **Enhanced Service Registration** with options delegate
- **Stable CRUD + Batch Operations** 

---

### ✨ Why IndexedDB.NET?

Working with IndexedDB directly in Blazor requires:

* JavaScript interop boilerplate
* Manual transaction handling
* Complex async patterns
* Error-prone schema management

---

## ✨ Features

✔ Full CRUD + Batch operations  
✔ Multi-entity support (multiple stores)  
✔ EF Core-style `DbContext` pattern  
✔ Robust migration & versioning system  
✔ `Guid`, `int`, `string` key support  
✔ Smart automatic `Id` handling  
✔ Safe transaction management  
✔ Production-ready stability  
✔ Lightweight & dependency-free  

---

## 📦 Installation

```bash
dotnet add package ManuHub.IndexedDB
```

---

## ⚙️ Setup

### 1. Register in `Program.cs`

```csharp
builder.Services.AddIndexedDb<AppDbContext>(options =>
{
    options.DatabaseName = "MyAppDb";
    options.Version = 5;                    // Increase when schema changes

    // Migrations (recommended)
    options.Migrations.Add(1, builder =>
    {
        builder.CreateStore<TodoItem>();
        builder.CreateStore<User>();
    });

    // Add more migrations as needed
});
```

### 2. Create DbContext

```csharp
public class AppDbContext : IndexedDbContext
{
    public AppDbContext(IndexedDbOptions options) : base(options) { }

    protected override IEnumerable<Type> GetEntityTypes()
    {
        return [typeof(TodoItem), typeof(User)];
    }

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("TodoItems");
    public IndexedSet<User> Users => Set<User>("Users");
}
```

### 3. Define Entities

```csharp
[IndexedStore("TodoItems")]
public class TodoItem
{
    [IndexedKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

---

## 🧪 Usage

### Basic Operations

```csharp
// Add
await db.Todos.AddAsync(new TodoItem { Title = "Learn ManuHub.IndexedDB" });

// Get All
var allTodos = await db.Todos.GetAllAsync();

// Query
var completed = await db.Todos.Query()
    .WhereEquals("IsDone", true)
    .ToListAsync();

// Update & Delete
await db.Todos.UpdateAsync(todo);
await db.Todos.DeleteAsync(todo.Id);
```

### Batch Operations
```csharp
await db.Todos.AddRangeAsync(items);
await db.Todos.UpdateRangeAsync(updatedItems);
await db.Todos.DeleteRangeAsync(idsToDelete);
```

---

## 🔄 Migrations (v1.2 Feature)

### Basic Migration Example

```csharp
builder.Services.AddIndexedDb<AppDbContext>(options =>
{
    options.DatabaseName = "MyAppDb";
    options.Version = 3;   // Current version

    options.Migrations.Add(1, builder =>
    {
        builder.CreateStore<TodoItem>();
        builder.CreateStore<User>();
    });

    options.Migrations.Add(2, builder =>
    {
        builder.CreateStore<Project>();
    });

    options.Migrations.Add(3, builder =>
    {
        builder.CreateStore<TodoItem>(); // Re-apply if needed
        // Optional: Data transformation
        // builder.TransformData<TodoItem>(todo => { ... });
    });
});
```

### Advanced Migration with Data Transformation

```csharp
options.Migrations.Add(4, builder =>
{
    builder.CreateStore<Order>();

    // Transform existing data during migration
    builder.TransformData<TodoItem>(todo =>
    {
        todo.UpdatedAt = DateTime.UtcNow;
        todo.Version = 2;
        return todo;
    });
});
```

---

## 🛠 Architecture

```
Blazor App
   ↓
IndexedDbContext
   ↓
IndexedSet<T> / IndexedQuery<T>
   ↓
Smart JS Interop Layer (Multi-entity safe)
   ↓
IndexedDB Browser API
```

---

## ⚙️ Key Improvements in v1.2

- Removed hardcoded `normalizeEntity` (Todo-only)
- Smart generic entity normalization
- Reliable database versioning (no more data loss on refresh)
- Better fallback for `Id` / `Guid` keys
- Stable migration system
- Improved error handling & logging

---

## 📄 License

MIT License

---

## 🤝 Contributing

Pull requests, bug reports, and feature suggestions are welcome.

---



