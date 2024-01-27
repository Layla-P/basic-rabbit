using Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddRabbitMQ("consumer");
builder.Services.AddHostedService<Listener>();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.



app.Run();



internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
