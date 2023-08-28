using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ClipUploader.Dtos;
using ClipUploader.Services;
using Microsoft.Extensions.Configuration;

namespace ClipUploaderTests.Services;
public class QueueServiceTests
{
    private QueueService _queueService;
    private QueueClient _queueClient;

    [SetUp]
    public void SetUp()
    {
        var inMemConfig = new Dictionary<string, string> {
            { "QueueName","clips" }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemConfig)
            .Build();

        var queueOpts = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };
        var connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        QueueServiceClient queueServiceClient = new QueueServiceClient(connectionString, queueOpts);


        _queueClient = queueServiceClient.GetQueueClient("clips");
        _queueService = new QueueService(config, queueServiceClient);
    }

    [Test]
    public async Task EnqueueTest()
    {
        var storageUri1 = "www.somestorage.com/clips/" + Guid.NewGuid().ToString();
        EnqueueRequestDto clip = new EnqueueRequestDto
        {
            UserId = "someFakeUserId",
            Name = "TestName2",
            Description = "Description",
            StorageUri = storageUri1,
        };

        var storageUri2 = "www.somestorage.com/clips/" + Guid.NewGuid().ToString();
        EnqueueRequestDto clip2 = new EnqueueRequestDto
        {
            UserId = "someFakeUserId2",
            Name = "TestName2",
            Description = "Description",
            StorageUri = storageUri2,
        };

        var result1 = await _queueService.Enqueue(clip);
        var result2 = await _queueService.Enqueue(clip2);

        Assert.IsFalse(result1.IsError);
        Assert.IsFalse(result2.IsError);

        PeekedMessage[] peakedMessages = await _queueClient.PeekMessagesAsync(maxMessages: 10);
        var foundMessage = peakedMessages.FirstOrDefault<PeekedMessage>(m => m.MessageId == result1.Result.Id);
        var foundMessage2 = peakedMessages.FirstOrDefault<PeekedMessage>(m => m.MessageId == result2.Result.Id);

        Assert.IsNotNull(foundMessage);
        Assert.IsNotNull(foundMessage2);
    }
}
