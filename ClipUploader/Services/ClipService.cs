using ClipUploader.Dtos;
using ClipUploader.Errors;
using ClipUploader.Models;
using Newtonsoft.Json;
using RestSharp;

namespace ClipUploader.Services;

public class ClipService : IClipService
{

    IConfiguration _config;

    public ClipService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<ServiceResult<AddClipResponseDto>> AddClipToUserAsync(AddClipRequestDto addClipRequestDto)
    {
        ServiceResult<AddClipResponseDto> serviceResult = new();
        try
        {
            // Get token
            var tokenEndpoint = Environment.GetEnvironmentVariable("Auth0TokenEndpoint");
            var tokenRestClient = new RestClient(tokenEndpoint);
            var tokenRequest = new RestRequest();

            tokenRequest.AddHeader("content-type", "application/json");
            tokenRequest.AddBody(new
            {
                client_id = Environment.GetEnvironmentVariable("Auth0ClientId"),
                client_secret = Environment.GetEnvironmentVariable("Auth0ClientSecret"),
                audience = Environment.GetEnvironmentVariable("Auth0Audience"),
                grant_type = "client_credentials"
            });

            var tokenResponse = await tokenRestClient.ExecutePostAsync(tokenRequest);

            if (tokenResponse.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(tokenResponse.StatusCode.ToString());

            // Get Clip
            var token = JsonConvert.DeserializeObject<TokenResponseDto>(tokenResponse.Content!);
            var baseUrl = Environment.GetEnvironmentVariable("ClipsServiceEndpoint");
            var uri = new Uri(Uri.EscapeUriString($"{baseUrl}/{addClipRequestDto.UserId}/clips"));
            var clipsClient = new RestClient(uri);

            var clipsRequest = new RestRequest();
            clipsRequest.AddHeader("content-type", "application/json");
            clipsRequest.AddHeader("Authorization", $"{token.TokenType} {token.AccessToken}");
            clipsRequest.AddBody(new
            {
                name = addClipRequestDto.Name,
                description = addClipRequestDto.Description,
                uri = "",
                converted = false,
                Public = addClipRequestDto.Public,
            });
            var clipsResponse = await clipsClient.ExecutePostAsync(clipsRequest);

            if (clipsResponse.StatusCode != System.Net.HttpStatusCode.OK)
                // @TODO - better execption handling here errormessage is null
                throw new Exception(clipsResponse.ErrorMessage.ToString());

            var addClipResponseDto = JsonConvert.DeserializeObject<AddClipResponseDto>(clipsResponse.Content!);

            if (addClipResponseDto?.ClipId == null)
                throw new Exception("There was a problem Adding a clip, reponse did not provide an Id.");

            serviceResult.Result = new AddClipResponseDto()
            {
                ClipId = addClipResponseDto.ClipId,
            };
        }
        catch (Exception ex)
        {
            serviceResult.IsError = true;
            serviceResult.ErrorMessage = ex.ToString();
        }

        return serviceResult;
    }
}
