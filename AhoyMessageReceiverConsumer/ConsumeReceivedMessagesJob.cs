using MassTransit;
using Quartz;

internal class ConsumeReceivedMessagesJob : IJob
{

    private readonly IBusControl _busControl;
    public ConsumeReceivedMessagesJob(IBusControl busControl)
    {
        _busControl = busControl;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"{DateTime.Now}: MyJob is executing.");
        _busControl.Start();
    }
}