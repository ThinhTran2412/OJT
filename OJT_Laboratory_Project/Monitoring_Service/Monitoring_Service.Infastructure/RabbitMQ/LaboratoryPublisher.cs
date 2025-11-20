using Monitoring_Service.Application.DTOs;
using Monitoring_Service.Application.Interface;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Monitoring_Service.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Implements the laboratory publisher.
    /// </summary>
    /// <seealso cref="Monitoring_Service.Application.Interface.ILaboratoryPublisher" />
    /// <seealso cref="System.IDisposable" />
    public class LaboratoryPublisher : ILaboratoryPublisher, IDisposable
    {
        /// <summary>
        /// The connection
        /// </summary>
        private readonly IConnection _connection;
        /// <summary>
        /// The channel
        /// </summary>
        private readonly IModel _channel;
        /// <summary>
        /// The queue name
        /// </summary>
        private const string QueueName = "monitoring.to.laboratory.rawresult";

        /// <summary>
        /// Initializes a new instance of the <see cref="LaboratoryPublisher"/> class.
        /// </summary>
        public LaboratoryPublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task PublishAsync(RawTestResultDTO message)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange: "",
                                  routingKey: QueueName,
                                  basicProperties: null,
                                  body: body);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
