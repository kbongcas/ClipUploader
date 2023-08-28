using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClipUploader.Dtos;
using ClipUploader.Errors;

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
            Environment.GetEnvironmentVariable("BlobContainerName"));
    }
    
    public async Task<ServiceResult<StorageUploadResponseDto>> UploadAsync(StorageUploadRequestDto storageUploadRequestDto)
    {
        ServiceResult<StorageUploadResponseDto> serviceResult = new();
        try
        {
            if (storageUploadRequestDto.File == null) 
                throw new Exception("Clip does not contain a file.");
            if (String.IsNullOrEmpty(storageUploadRequestDto.Id)) 
                throw new Exception("Clip does not contain an Id.");

            BlobClient blobClient = _blobContainerClient.GetBlobClient(storageUploadRequestDto.Id);
            Azure.Response<BlobContentInfo> response;
            await using (Stream? stream = storageUploadRequestDto.File.OpenReadStream())
            {
                response = await blobClient.UploadAsync(stream);
            }

            if (response.GetRawResponse().IsError) 
                throw new Exception(response.GetRawResponse().ReasonPhrase);

            serviceResult.Result = new StorageUploadResponseDto()
            {
                StorageUri = blobClient.Uri.AbsoluteUri
            };
        }
        catch(Exception ex)
        {
            serviceResult.IsError = true;
            serviceResult.ErrorMessage = ex.Message;
        }

        return serviceResult;
    }
}
