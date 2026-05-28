using IndexedDB.NET.Wasm;
using ManuHub.IndexedDB;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddIndexedDb<AppDbContext>(options =>
{
    options.DatabaseName = "MyAppDb";
    options.Version = 10;

    options.Migrations.Add(1, builder =>
    {
        builder.CreateStore<TodoItem>();
        builder.CreateStore<Employee>();
    });    
});

await builder.Build().RunAsync();
