using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MediatR;
using Monitoring_Service.Application.HandleRawTestResult.Command;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Monitoring_Service.Infastructure.RabbitMQ
{
    /// <summary>
    /// Creates the rabbit mq consumer.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class RabbitMQConsumer : BackgroundService
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
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// The queue name
        /// </summary>
        private const string QueueName = "raw_test_result_queue";

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQConsumer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public RabbitMQConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.
        /// </returns>
        /// <remarks>
        /// See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.
        /// </remarks>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Gửi message sang Handler để xử lý
                await mediator.Send(new HandleRawTestResultCommand(message));

                // Xác nhận đã xử lý xong
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        /// <summary>
        /// </summary>
        /// <inheritdoc />
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
