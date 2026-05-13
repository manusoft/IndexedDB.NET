﻿![Static Badge](https://img.shields.io/badge/ManuHub.IndexedDB-blue) ![NuGet Version](https://img.shields.io/nuget/v/ManuHub.IndexedDB)  ![NuGet Downloads](https://img.shields.io/nuget/dt/ManuHub.IndexedDB)

# 📦 IndexedDB.NET

> **v3.0**  
> A lightweight, EF Core–style IndexedDB wrapper for **Blazor WebAssembly**

---

## 🚀 Overview

**IndexedDB.NET** is a .NET-first abstraction over the browser’s IndexedDB API, designed specifically for **Blazor WebAssembly** applications.

It provides a clean, strongly-typed, EF Core–inspired API to manage client-side storage without writing JavaScript.

---

### ✨ Why IndexedDB.NET?

Working with IndexedDB directly in Blazor requires:

* JavaScript interop boilerplate
* Manual transaction handling
* Complex async patterns
* Error-prone schema management

**IndexedDB.NET solves this by providing:**

✔ EF Core–like `DbContext` experience  
✔ Strongly typed stores (`IndexedSet<T>`)  
✔ Clean CRUD + batch operations  
✔ Safe JS interop layer  
✔ Automatic schema initialization  
✔ Blazor-first architecture  

---

## 📦 Features (V1)

### Core Features

* ✔ Full CRUD operations
* ✔ Batch insert / update / delete
* ✔ Typed DbContext pattern
* ✔ IndexedDB schema configuration
* ✔ Key-based entity storage
* ✔ GUID / string key support
* ✔ JSInterop abstraction layer

### Batch Operations

* AddRange
* UpdateRange
* DeleteRange

### Reliability

* Safe transaction handling
* DB connection lifecycle management
* Version-based schema upgrades
* Indexed store validation

---

## ⚙️ Installation

```bash
dotnet add package IndexedDB.NET
```

---

## 🚀 Quick Start

---

### 1. Register DbContext

```csharp
builder.Services.AddScoped<AppDbContext>();
```

---

### 2. Create Your DbContext

```csharp
public class AppDbContext : IndexedDbContext
{
    public AppDbContext(IJSRuntime js) : base(js)
    {
    }

    protected override IndexedDbOptions OnConfiguring()
    {
        return new IndexedDbOptions
        {
            DatabaseName = "SampleDB",
            Version = 1,
            Stores =
            {
                new StoreSchema
                {
                    Name = "todos",
                    KeyPath = "Id",
                    AutoIncrement = false,

                    Indexes =
                    {
                        new IndexSchema
                        {
                            Name = "Title",
                            KeyPath = "Title"
                        },
                        new IndexSchema
                        {
                            Name = "IsDone",
                            KeyPath = "IsDone"
                        }
                    }
                }
            }
        };
    }

    public IndexedSet<TodoItem> Todos => Set<TodoItem>("todos");
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

## 🧪 Usage Examples

---

### ➕ Add Item

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

---

### ➕ Add Multiple

```csharp
await Db.Todos.AddRangeAsync(items);
```

---

### ✏️ Update Multiple

```csharp
await Db.Todos.UpdateRangeAsync(items);
```

---

### ❌ Delete Multiple

```csharp
await Db.Todos.DeleteRangeAsync(items.Select(x => x.Id));
```

---

## 🧠 Architecture

IndexedDB.NET follows a clean layered design:

```
Blazor WebAssembly App
        ↓
AppDbContext (C# abstraction)
        ↓
IndexedSet<T> (typed store API)
        ↓
JS Interop Layer
        ↓
IndexedDB Browser API
```

---

## ⚠️ Version 1 Limitations

This release focuses on **core stability and CRUD operations**.

### Not yet supported:

* LINQ-style queries (`Where`, `Select`)
* Change tracking / `SaveChanges()`
* Navigation properties / relationships
* Transactions API abstraction layer
* Offline sync (REST / MongoDB)
* Aggregations & grouping
* Advanced query planning
* Multi-database orchestration
* Automatic conflict resolution

---

## 🧭 Roadmap (V2)

Planned improvements:

* LINQ expression queries
* EF Core–style change tracking
* Unit-of-work (`SaveChanges`)
* Offline sync engine (REST / MongoDB)
* Query optimizer & index selection
* Migration framework
* Conflict resolution strategies
* Streaming / cursor-based APIs

---

## 💡 Design Philosophy

> “Bring EF Core simplicity to browser storage — without the weight.”

IndexedDB.NET is designed to:

* feel familiar to .NET developers
* hide JavaScript complexity
* remain lightweight and fast
* work natively in Blazor WebAssembly

---

## 🛠 Tech Stack

* Blazor WebAssembly
* IndexedDB (Browser API)
* JavaScript ES Modules
* .NET JSInterop
* C# generics + attributes

---

## 🤝 Contributing

Contributions are welcome.

This project is evolving toward a full **client-side ORM for Blazor WASM**.

---

## 📄 License

MIT License

---

## ⭐ Status

| Property  | Value                     |
| --------- | ------------------------- |
| Version   | 1.0.0                     |
| Stability | Production (Core CRUD)    |
| Target    | Blazor WebAssembly        |
| Scope     | Client-side IndexedDB ORM |

---

## 🔥 Final Note

If EF Core is for SQL Server…

> IndexedDB.NET is for the browser.

