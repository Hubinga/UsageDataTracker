using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartMeterApp;
using SmartMeterApp.Services;
using SmartMeterApp.Utility;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

/*Sicherheitsprinzipien: 
 - Verwendung von HTTPS stellt sicher, dass die Kommunikation verschlüsselt ist und vor Abhören und Manipulation geschützt wird.
 - HTTPS verschlüsselt die Daten während der Übertragung, wodurch sie vor Abhören und Manipulation durch Dritte (z.B. Man-in-the-Middle-Angriffe) geschützt sind.*/

// Configure HttpClient to send requests to the API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7114/") });

// Add service for displaying Toast messages
builder.Services.AddSingleton<ToastService>();
// Add service to notify the Chart to update if user edited a usage data value
builder.Services.AddSingleton<IEventAggregator, EventAggregator>();

await builder.Build().RunAsync();
