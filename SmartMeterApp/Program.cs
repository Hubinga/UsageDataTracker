using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SmartMeterApp;
using SmartMeterApp.Services;
using SmartMeterApp.Utility;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Configure HttpClient to send requests to the API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7114/") });

builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<IEventAggregator, EventAggregator>();

await builder.Build().RunAsync();
