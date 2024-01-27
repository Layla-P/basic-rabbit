using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQ("producer");

var app = builder.Build();

app.MapDefaultEndpoints();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/hello", () => "Hi I am the Producer");

app.MapGet("/weatherforecast", (IConnection connection) =>
{
	var forecast = Enumerable.Range(1, 5).Select(index =>
		new WeatherForecast
		(
			DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			Random.Shared.Next(-20, 55),
			summaries[Random.Shared.Next(summaries.Length)]
		))
		.ToArray();

	var channel = connection.CreateModel();
	channel.ExchangeDeclare("basic_topic", "topic");
	var message = JsonSerializer.Serialize(forecast);
	//topic should become enum or similar
	var body = Encoding.UTF8.GetBytes(message);
	channel.BasicPublish("basic_topic", "rabbit.message", null, body);
	Console.WriteLine("Message sent");

	return "Message Sent";
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
