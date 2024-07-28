using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartMeterApp;
using SmartMeterApp.Services;
using SmartMeterApp.Utility;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

/*Sicherheitsprinzipien: 
 - Verwendung von HTTPS stellt sicher, dass die Kommunikation verschl�sselt ist und vor Abh�ren und Manipulation gesch�tzt wird.
 - HTTPS verschl�sselt die Daten w�hrend der �bertragung, wodurch sie vor Abh�ren und Manipulation durch Dritte (z.B. Man-in-the-Middle-Angriffe) gesch�tzt sind.*/

// Configure HttpClient to send requests to the API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7114/") });

// Add service for displaying Toast messages
builder.Services.AddSingleton<ToastService>();
// Add service to notify the Chart to update if user edited a usage data value
builder.Services.AddSingleton<IEventAggregator, EventAggregator>();

await builder.Build().RunAsync();
