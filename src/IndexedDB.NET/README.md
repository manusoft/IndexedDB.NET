
﻿﻿![Static Badge](https://img.shields.io/badge/ManuHub.IndexedDB-blue) ![NuGet Version](https://img.shields.io/nuget/v/ManuHub.IndexedDB)  ![NuGet Downloads](https://img.shields.io/nuget/dt/ManuHub.IndexedDB)

# 📦 ManuHub.IndexedDB

> **v1.1**  
A lightweight, production-ready **IndexedDB wrapper for Blazor WebAssembly** with a clean EF-Core-inspired API.

---

## 🚀 Overview

**ManuHub.IndexedDB** provides a simple and strongly structured API for working with browser IndexedDB in Blazor WASM applications.

It abstracts JavaScript interop complexity and offers a clean .NET-first developer experience.

---

## ♻️ Changelog v1.1.0

### Added
- Added `AddIndexedDb<TContext>()` service registration extension

### Improved
- Simplified `IndexedDbContext` setup
- Improved dependency injection experience
- Reduced manual `IJSRuntime` boilerplate
- Improved EF Core–style architecture and usability

---

## ✨ Features (V1)

✔ Full CRUD support (Add / Get / Update / Delete)   
✔ Batch operations (AddRange / UpdateRange / DeleteRange)  
✔ EF-style `DbContext` pattern  
✔ Clean JS interop abstraction   
✔ IndexedDB schema initialization  
✔ Safe transaction handling  
✔ GUID/string key support  
✔ Blazor WebAssembly optimized  
✔ Lightweight & dependency-free  

---

## 📦 Installation

```bash
dotnet add package ManuHub.IndexedDB
```

---

## ⚙️ Setup

### 1. Register DbContext

```csharp
builder.Services.AddIndexedDb<AppDbContext>();
```

---

### 2. Create DbContext

```csharp
public class AppDbContext : IndexedDbContext
{
    public AppDbContext()
        : base(new IndexedDbOptions
        {
            DatabaseName = "SampleDB",
            Version = 1
        })
    {
    }

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("todos");
}
```

**OR**

```csharp
public class AppDbContext : IndexedDbContext
{
    public AppDbContext(ILogger<AppDbContext> logger)
        : base(new IndexedDbOptions
        {
            DatabaseName = "SampleDB",
            Version = 1
        }, logger)
    {
    }

    // -----------------------------------------------------
    // ENTITY REGISTRY
    // -----------------------------------------------------

    protected override IEnumerable<Type> GetEntityTypes()
    {
        yield return typeof(TodoItem);

        // Add more entity types here as needed
    }

    // -----------------------------------------------------
    // STORES
    // -----------------------------------------------------

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("todos");

    // Add more stores here as needed

    // -----------------------------------------------------
    // OPTIONAL: Query access (clean API)
    // -----------------------------------------------------
    public IndexedQuery<TodoItem> TodoQuery => Query<TodoItem>("todos");

    // Add more queries here as needed
}
```

---

### 3. Define Model

```csharp
[IndexedStore("todos")]
public class TodoItem
{
    [IndexedKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = "";

    public bool IsDone { get; set; }
}
```

---

## 🧪 Basic Usage

### ➕ Add

```csharp
await Db.Todos.AddAsync(new TodoItem
{
    Title = "Learn IndexedDB.NET"
});
```

---

### 📖 Get All

```csharp
var items = await Db.Todos.GetAllAsync();
```

---

### ✏️ Update

```csharp
item.Title = "Updated";
await Db.Todos.UpdateAsync(item);
```

---

### ❌ Delete

```csharp
await Db.Todos.DeleteAsync(item.Id);
```

---

## ⚡ Batch Operations

### ➕ Add Range

```csharp
await Db.Todos.AddRangeAsync(items);
```

---

### ✏️ Update Range

```csharp
await Db.Todos.UpdateRangeAsync(items);
```

---

### ❌ Delete Range

```csharp
await Db.Todos.DeleteRangeAsync(items.Select(x => x.Id));
```

---

## 🧠 Architecture

ManuHub.IndexedDB uses a layered design:

```
Blazor App
   ↓
IndexedDbContext (C#)
   ↓
IndexedSet<T>
   ↓
JS Interop Layer
   ↓
IndexedDB Browser API
```

---

## ⚠️ Version 1 Limitations

This is a **V1 release focused on core stability**.

### Not included yet:

* LINQ-style querying (`Where`, `Select`)
* Change tracking / `SaveChanges()`
* Relationships / navigation properties
* Transactions API abstraction
* Offline sync (MongoDB / REST)
* Advanced indexing strategy
* Aggregations / grouping
* Multi-database orchestration

---

## 🧭 Roadmap (V2)

Planned improvements:

* LINQ expression support
* EF Core-style change tracking
* Unit-of-work pattern
* Offline sync engine (REST / MongoDB)
* Query optimizer & index selection
* Migration framework
* Conflict resolution strategies

---

## 💡 Design Philosophy

> “Make browser storage feel like EF Core — but lightweight, fast, and Blazor-native.”

---

## 🛠 Tech Stack

* Blazor WebAssembly
* IndexedDB (Browser API)
* JavaScript ES Modules
* .NET JSInterop

---

## 📄 License

MIT License

---

## 🤝 Contributing

Pull requests and suggestions are welcome.
This project is designed to evolve toward a full EF-Core-like IndexedDB ORM.

---

## ⭐ Status

**Version:** 1.1.0  
**Stability:** Production-ready (core CRUD)  
**Target:** Blazor WebAssembly applications  



