using Azure;
using ClipUploader.Dtos;
using ClipUploader.Models;
using ClipUploader.Services;
using Microsoft.Extensions.Configuration;

namespace ClipUploaderTests.Services;
public class ClipServiceTests
{
    // @TODO DONOT PUT INTO VC
    private readonly string tokenEndpoint = "";
    private readonly string clientId = "";
    private readonly string clientSecret = "";
    private readonly string audience = "";
    private readonly string clipsServiceEndpoint = "";
    private readonly string userId = "";

    private ClipService _clipService;

    [SetUp]
    public void SetUp()
    {
        var inMemConfig = new Dictionary<string, string> {
            {"Auth0TokenEndpoint", tokenEndpoint},
            {"Auth0ClientId", clientId },
            {"Auth0ClientSecret", clientSecret},
            {"Auth0Audience", audience},
            {"ClipsServiceEndpoint", clipsServiceEndpoint }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemConfig)
            .Build();

        _clipService = new ClipService(config);
    }

    [Test]
    public async Task AddClipToUserTest()
    {
        var addClipRequestDto = new AddClipRequestDto
        {
            Name = "test",
            Description = "description",
            UserId = userId,
        };
        var response = await _clipService.AddClipToUserAsync(addClipRequestDto);
        Assert.IsNotNull(response);
        Assert.IsTrue(Guid.TryParse(response.Result.ClipId, out _));
    }
}
