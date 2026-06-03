using System.Text;
using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TapAi.Module.Identity.Domain.Events;
using TapAi.Module.Identity.Infrastructure.Options;

namespace TapAi.Module.Identity.Infrastructure.EmailWorker;

public sealed class EmailConsumerWorker(
    IOptions<RabbitMqOptions> rabbitOptions,
    IOptions<EmailOptions> emailOptions,
    ILogger<EmailConsumerWorker> logger
) : BackgroundService
{
    private readonly RabbitMqOptions _rabbit = rabbitOptions.Value;
    private readonly EmailOptions _email = emailOptions.Value;

    private const string QueueName = "user_registered";
    private const int MaxConnectionAttempts = 10;

    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // RabbitMQ hazır olmasa (Docker Compose race) retry ilə gözlə
        var factory = new ConnectionFactory
        {
            HostName = _rabbit.Host,
            Port = _rabbit.Port,
            VirtualHost = _rabbit.VirtualHost,
            UserName = _rabbit.Username,
            Password = _rabbit.Password
        };

        for (int attempt = 1; attempt <= MaxConnectionAttempts; attempt++)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(30, attempt * 3));
                logger.LogWarning(ex,
                    "RabbitMQ bağlantı cəhdi {Attempt}/{Max} uğursuz oldu. {Delay}s gözlənilir.",
                    attempt, MaxConnectionAttempts, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
        }

        if (_connection is null)
        {
            logger.LogCritical(
                "RabbitMQ-ya {Max} cəhddən sonra qoşulmaq mümkün olmadı. " +
                "Email worker işə başlamayacaq.", MaxConnectionAttempts);
            return;
        }

        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(body);

                if (evt is not null)
                    await SendWelcomeEmailAsync(evt, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Xoş gəldin emaili göndərilə bilmədi. Mesaj rədd edilir (requeue: false).");
                // requeue: false — poison message-ı sonsuz loop-a salmaq əvəzinə at;
                // DLX konfiqurasiyası varsa ora yönləndiriləcək.
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task SendWelcomeEmailAsync(UserRegisteredEvent evt, CancellationToken ct)
    {
        // TODO: SMS module hazır olduqda burada xoş gəldin SMS-i göndəriləcək.
        logger.LogInformation(
            "İstifadəçi qeydiyyatdan keçdi: UserId={UserId}, Ad={FirstName} {LastName}",
            evt.UserId, evt.FirstName, evt.LastName);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
