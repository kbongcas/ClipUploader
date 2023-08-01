using ClipUploader.Dtos;

namespace ClipUploader.Services;

public interface IStorageService
{
    Task<BlobResponseDto> UploadAsync(IFormFile formFile);
}