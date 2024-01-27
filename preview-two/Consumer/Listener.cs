using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Consumer;

public class Listener(IConnection connection) : IHostedService
{
	public Task StartAsync(CancellationToken cancellationToken)
	{
		DoStuff();
		return Task.CompletedTask;

	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private void DoStuff()
	{

		var channel = connection.CreateModel();
		channel.ExchangeDeclare("basic_topic", "topic");
		var queue = channel.QueueDeclare("weather_queue");
		channel.QueueBind(queue, "basic_topic", "rabbit.message");
		var consumerAsync = new AsyncEventingBasicConsumer(channel);
		consumerAsync.Received += async (_, ea) =>
		{
			Console.WriteLine("Message received");
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			var wf = JsonSerializer.Deserialize<WeatherForecast[]>(message);

			channel.BasicAck(ea.DeliveryTag, false);

			foreach (var w in wf)
			{
				Console.WriteLine($"Day: {w.Date.Day}, Summary: {w.Summary}");
			}
			await Task.CompletedTask;
		};

		channel.BasicConsume(queue, false, consumerAsync);
	}
}
