using Boundless.Backend;
using static Boundless.Backend.Logging;

StartLogging("Boundless.Backend-.log");
Configuration.Initialize();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

