using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClipUploader.Dtos;
using ClipUploader.Models;

namespace ClipUploader.Services;

public class StorageService : IStorageService
{
    private BlobServiceClient _blobServiceClient;
    private BlobContainerClient _blobContainerClient;

    public StorageService(
        IConfiguration config,
        BlobServiceClient blobServiceClient 
        )
    {
        _blobServiceClient = blobServiceClient;
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(
            config.GetValue<string>("BlobContainerName"));
    }
    
    public async Task<BlobResponseDto> UploadAsync(Clip clip)
    {
        BlobResponseDto responseDto = new BlobResponseDto();
        try
        {
            if (clip.File == null) 
                throw new Exception("Clip does not contain a file.");

            BlobClient blobClient = _blobContainerClient.GetBlobClient(clip.Id);
            Azure.Response<BlobContentInfo> response;
            await using (Stream? stream = clip.File.OpenReadStream())
            {
                response = await blobClient.UploadAsync(stream);
            }

            if (response.GetRawResponse().IsError) 
                throw new Exception(response.GetRawResponse().ReasonPhrase);

            responseDto.Clip = clip;
            responseDto.Clip.Uri = blobClient.Uri.AbsoluteUri;
        }
        catch(Exception ex)
        {
            //@TODO - Log Error Message
            responseDto.ErrorMessage = ex.Message;
        }
        return responseDto;
    }
}
