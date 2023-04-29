using System.Net;
using System.Text.Json;
using AhoyContracts.Messages;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AhoyMessageSender;

public class Consumer : BackgroundService
{
    private readonly ILogger<Consumer> _logger;
    private readonly HttpClient _whatsAppConnectorClient;
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueName;
    private string _currentReceiptHandle;

    public Consumer(ILogger<Consumer> logger, IConfiguration configuration, IAmazonSQS sqsClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _whatsAppConnectorClient = httpClientFactory.CreateClient("AhoyWhatsAppConnector");
        _queueName = configuration["AWS:SQS:QueueName"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"{DateTime.Now}: MessageSender Job is executing.");
        var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName, cancellationToken);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            MaxNumberOfMessages = 5,
            WaitTimeSeconds = 10
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var messageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, cancellationToken);

                if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
                    continue;

                foreach (var message in messageResponse.Messages)
                {
                    Console.WriteLine($"Received message: {JsonSerializer.Serialize(message.Body, new JsonSerializerOptions { WriteIndented = true })}");
                    
                    _currentReceiptHandle = message.ReceiptHandle;

                    var outgoingChatMessage = JsonSerializer.Deserialize<OutgoingMessage>(message.Body);

                    await _whatsAppConnectorClient.PostAsJsonAsync("message/text", new { phoneNumber = outgoingChatMessage.CustomerContact, message = outgoingChatMessage.Content.Text });

                    await _sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Error processing message: {Message}", exception.Message);
            }
        }
    }
}