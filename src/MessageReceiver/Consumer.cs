using System.Net;
using System.Text;
using System.Text.Json;
using AhoyContracts.Messages;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AhoyMessageReceiver;

internal class Consumer : BackgroundService
{
    private readonly ILogger<Consumer> _logger;
    private readonly HttpClient _ahoyChatClient;
    private readonly IAmazonSQS _sqsClient;
    // private readonly List<string> _messageAttributeNames = new() { "All" };
    // private readonly List<string> _attributeNames = new() { "All" };
    private readonly string _queueName;
    private string _currentReceiptHandle;

    public Consumer(ILogger<Consumer> logger, IConfiguration configuration, IAmazonSQS sqsClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _ahoyChatClient = httpClientFactory.CreateClient("AhoyChat");
        _queueName = configuration["AWS:SQS:QueueName"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"{DateTime.Now}: MessageReceiver Job is executing.");
        var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName, cancellationToken);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            // MessageAttributeNames = _messageAttributeNames,
            // AttributeNames = _attributeNames,
            MaxNumberOfMessages = 5,
            WaitTimeSeconds = 10
        };

        while(!cancellationToken.IsCancellationRequested)
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

                    //TODO: search for a user
                    var userId = "7140ca1a-af0f-4ce5-91d0-e3cf7e262da0";

                    await _ahoyChatClient.PostAsync($"message/{userId}", new StringContent(message.Body, Encoding.UTF8, "application/json") );

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