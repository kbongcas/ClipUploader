using ClipUploader.Dtos;
using ClipUploader.Errors;

namespace ClipUploader.Services;
public interface IClipService
{
    Task<ServiceResult<AddClipResponseDto>> AddClipToUserAsync(AddClipRequestDto addClipRequestDto);
}