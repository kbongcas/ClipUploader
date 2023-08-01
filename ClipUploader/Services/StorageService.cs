using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClipUploader.Dtos;

namespace ClipUploader.Services;

public class StorageService : IStorageService
{
    private IConfiguration _config;
    private BlobServiceClient _blobServiceClient;
    private BlobContainerClient _blobContainerClient;

    public StorageService(
        IConfiguration config,
        BlobServiceClient blobServiceClient 
        )
    {
        _config = config;
        _blobServiceClient = blobServiceClient;
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(
            _config.GetValue<string>("BlobContainerName"));
    }
    
    public async Task<BlobResponseDto> UploadAsync(IFormFile data)
    {
        BlobResponseDto responseDto = new BlobResponseDto();
        try
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(data.FileName);
            Azure.Response<BlobContentInfo> response;
            await using (Stream? stream = data.OpenReadStream())
            {
                response = await blobClient.UploadAsync(stream);
            }
            if(response.GetRawResponse().IsError) {
                //@TODO - Log Error Message
                responseDto.ErrorMessage = response.GetRawResponse().ReasonPhrase;
            }
            else
            {
                responseDto.Blob.Uri = blobClient.Uri.AbsoluteUri;
                responseDto.Blob.Name = blobClient.Name;
            }
        }
        catch(Exception ex)
        {
            responseDto.ErrorMessage = ex.Message;
        }
        return responseDto;
    }
}
