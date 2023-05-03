using System.Text.Json;
using AhoyContracts.Messages;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AhoyChat;

public class Publisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueName;

    public Publisher(IAmazonSQS sqsClient, string queueName)
    {
        _sqsClient = sqsClient;
        _queueName = queueName;
    }

    public async Task Publish(OutgoingMessage chatMessage, CancellationToken cancellationToken = default)
    {
        var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName, cancellationToken);

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            MessageBody = JsonSerializer.Serialize(chatMessage)
        };

        await _sqsClient.SendMessageAsync(request, cancellationToken);
    }
}
