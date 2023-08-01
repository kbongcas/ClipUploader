using ClipUploader.Dtos;
using ClipUploader.Models;

namespace ClipUploader.Services;
public interface IQueueService
{
    Task<QueueMessageResponseDto> Enqueue(Clip clip);
}