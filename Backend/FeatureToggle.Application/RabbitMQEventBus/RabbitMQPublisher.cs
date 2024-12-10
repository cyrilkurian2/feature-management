using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FeatureToggle.Application.RabbitMQEventBus
{
    public class RabbitMQPublisher(string exchangeName) : IEventBus
    {
        private string _exchangeName = exchangeName;
        private IConnection _connection;
        private IChannel _channel;

        public async Task InitializeAsync(string connectionString)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct);

            await _channel.QueueDeclareAsync(
                    queue: "feature-update-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false
            );

            await _channel.QueueBindAsync(
                queue: "feature-update-queue",
                exchange: _exchangeName,
                routingKey: "feature.update"
            );

        }

        public async Task Publish(string routingKey, object @event)
        {
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(exchange: _exchangeName, routingKey: routingKey, body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
