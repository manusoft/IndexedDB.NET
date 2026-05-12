using IndexedDB.NET.Wasm;
using ManuHub.IndexedDB;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddIndexedDb(options =>
//{
//    options.DatabaseName = "SampleDb";
//    options.Version = 1;
//});

builder.Services.AddScoped<AppDbContext>();

await builder.Build().RunAsync();
