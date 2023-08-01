using Azure.Storage.Blobs;
using ClipUploader.Dtos;
using ClipUploader.Models;
using ClipUploader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace ClipDataServiceTests.Services;
public class StorageServiceTests
{
    private StorageService _storageService;
    private BlobServiceClient _blobServiceClient;
    private readonly string containerName = "clips";


    [SetUp]
    public void SetUp()
    {
        // local storage account    
        var connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        _blobServiceClient = new BlobServiceClient(connectionString);

        var inMemConfig = new Dictionary<string, string> {
            {"BlobContainerName", containerName}
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemConfig)
            .Build();
        _storageService = new StorageService(config, _blobServiceClient);

    }

    [Test]
    public async Task UploadTest()
    {
        var fileName = "gs.mp4";
        var filePath = Path.Combine(Environment.CurrentDirectory, "Data/") + fileName;
        var Id = Guid.NewGuid().ToString();

        BlobResponseDto blobResponse = null;
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blob = containerClient.GetBlobClient(fileName);

        if(blob.Exists()) await blob.DeleteAsync();

        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
            var clip = new Clip { File = file, Name = fileName, Id = Id };
            blobResponse = await _storageService.UploadAsync(clip);
            blob = containerClient.GetBlobClient(Id);
        }

        Assert.True(blob.Exists());
        Assert.IsNull(blobResponse.ErrorMessage);
        Assert.That(blob.Uri.AbsoluteUri, Is.EqualTo(blobResponse.Clip.Uri));

    }
}