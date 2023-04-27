using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AhoyShared.Configuration;

public static class SqsConfiguration
{
    public static void AddSqs(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
    {
        var awsOptions = configuration.GetAWSOptions();

        awsOptions.Credentials = new BasicAWSCredentials(configuration["AWS:AccessKeyId"], configuration["AWS:SecretAccessKey"]);
        awsOptions.Region = RegionEndpoint.GetBySystemName(configuration["AWS:DefaultRegion"]);
        awsOptions.DefaultClientConfig.ServiceURL = configuration["LocalStack:ServiceUrl"];
        
        services.AddAWSService<IAmazonSQS>(awsOptions);

        // services.AddAWSService<IAmazonSQS>(new Amazon.Extensions.NETCore.Setup.AWSOptions
        // {
        //     Profile = "localstack",
        //     Region = Amazon.RegionEndpoint.USEast1
        // });

        //var queueName = configuration["AWS:SQS:QueueName"]!;

/*
        //Amazon.Runtime.FallbackCredentialsFactory.Reset();

        var accessKeyId = isDevelopment ? "test" : configuration["AWS:AccessKeyId"]!;
        var secretAccessKey = isDevelopment ? "test" : configuration["AWS:SecretAccessKey"]!;
        var region = configuration["AWS:DefaultRegion"]!;
        var localstackUrl = configuration["LocalStack:ServiceUrl"]!;
        var queueName = configuration["AWS:SQS:QueueName"]!;

        //Environment.SetEnvironmentVariable("AWS_EC2_METADATA_DISABLED", "true");
        // Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "test");
        // Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "test");

        var awsOptions = new AWSOptions
        {
            //Credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey),
            //Region = RegionEndpoint.GetBySystemName(region),
        };

        if (isDevelopment)
        {
            awsOptions.DefaultClientConfig.ServiceURL = "http://localhost:4566";//localstackUrl;
            awsOptions.DefaultClientConfig.UseHttp = true;
            //awsOptions.DefaultClientConfig.AuthenticationRegion = region;
        }

        services.AddAWSService<IAmazonSQS>(awsOptions);

*/

        /// CREATE QUEUE
        var serviceProvider = services.BuildServiceProvider();
        var sqsClient = serviceProvider.GetRequiredService<IAmazonSQS>();
        var createQueueRequest = new CreateQueueRequest { QueueName = configuration["AWS:SQS:QueueName"] };
        sqsClient.CreateQueueAsync(createQueueRequest).GetAwaiter().GetResult();

        // if (isDevelopment)
        // {
        //     var serviceProvider = services.BuildServiceProvider();
        //     var sqsClient = serviceProvider.GetRequiredService<IAmazonSQS>();
        //     var createQueueRequest = new CreateQueueRequest { QueueName = queueName };
        //     sqsClient.CreateQueueAsync(createQueueRequest).GetAwaiter().GetResult();
        // }
    }
}
