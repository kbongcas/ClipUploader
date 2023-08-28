using ClipUploader.Dtos;
using ClipUploader.Errors;

namespace ClipUploader.Services;
public interface IQueueService
{
    Task<ServiceResult<EnqueueResponseDto>> Enqueue(EnqueueRequestDto enqueueRequestDto);
}