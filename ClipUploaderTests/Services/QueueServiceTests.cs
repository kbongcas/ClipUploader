using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ClipUploader.Dtos;
using ClipUploader.Models;
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
        var id = Guid.NewGuid().ToString();
        var uri = "http://127.0.0.1:10000/devstoreaccount1/clips/" + id;
        Clip clip = new Clip
        {
            Id = id,
            Name = "TestName2",
            Uri = uri,
        };

        var id2 = Guid.NewGuid().ToString();
        var uri2 = "http://127.0.0.1:10000/devstoreaccount1/clips/" + id2;
        Clip clip2 = new Clip
        {
            Id = id2,
            Name = "TestName2",
            Uri = uri2,
        };

        var response = await _queueService.Enqueue(clip);
        var response2 = await _queueService.Enqueue(clip2);

        PeekedMessage[] peakedMessages = await _queueClient.PeekMessagesAsync(maxMessages: 10);
        var foundMessage = peakedMessages.FirstOrDefault<PeekedMessage>(m => m.MessageText == uri);
        var foundMessage2 = peakedMessages.FirstOrDefault<PeekedMessage>(m => m.MessageText == uri2);

        Assert.IsNotNull(foundMessage);
        Assert.IsNotNull(foundMessage2);
    }
}
