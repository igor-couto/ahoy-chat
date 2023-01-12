using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Quartz;
using Quartz.Impl;
using AhoyMessageReceiverConsumer;

var services = new ServiceCollection();
services.AddScoped<ConsumeReceivedMessagesJob>();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var accessKey = config.GetValue<string>("AWS:AccessKey");
var secretKey = config.GetValue<string>("AWS:SecretKey");

services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("https://sqs.us-east-1.amazonaws.com", sqsHostConfiguration =>
        {
            sqsHostConfiguration.AccessKey(accessKey);
            sqsHostConfiguration.SecretKey(secretKey);
        });
    });
    x.AddConsumer<ReceiverConsumer>();
});
var serviceProvider = services.BuildServiceProvider();

var schedulerFactory = new StdSchedulerFactory();

var scheduler = await schedulerFactory.GetScheduler();

var job = JobBuilder.Create<ConsumeReceivedMessagesJob>()
    .WithIdentity("ConsumeReceivedMessagesJob")
    .Build();

var trigger = TriggerBuilder.Create()
    .WithIdentity("EveryMinuteTrigger")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInMinutes(1)
        .RepeatForever())
    .Build();

await scheduler.ScheduleJob(job, trigger);

await scheduler.Start();

await scheduler.Shutdown();

public partial class Program { }