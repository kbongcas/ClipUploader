using ClipUploader.Dtos;
using ClipUploader.Errors;

namespace ClipUploader.Services;

public interface IStorageService
{
    Task<ServiceResult<StorageUploadResponseDto>> UploadAsync(StorageUploadRequestDto storageUploadRequestDto);
}