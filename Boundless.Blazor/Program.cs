using System.Diagnostics;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Boundless.Blazor;
using Boundless.Blazor.Accounts.Fortnite;

var auth = await FortniteHandler.GetDeviceAuthAsync();
Process.Start(new ProcessStartInfo
{
    FileName = auth.Uri,
    UseShellExecute = true
});




var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();