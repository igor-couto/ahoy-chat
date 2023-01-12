using MassTransit;

namespace AhoyMessageReceiverConsumer;

internal class ReceiverConsumer : IConsumer<AhoyMessage>
{
    public async Task Consume(ConsumeContext<AhoyMessage> context)
    {
        var message = context.Message;
        Console.WriteLine($"Received message: {message.Text}");
    }
}
