using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ClipUploader.Dtos;
using ClipUploader.Errors;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;

namespace ClipUploader.Services;

public class QueueService : IQueueService
{

    private QueueClient _queueClient;
    private QueueServiceClient _queueServiceClient;

    public QueueService(
        IConfiguration config,
        QueueServiceClient queueServiceClient
        )
    {
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(config.GetValue<string>("QueueName"));
    }

    public async Task<ServiceResult<EnqueueResponseDto>> Enqueue(EnqueueRequestDto enqueueRequestDto)
    {
        ServiceResult<EnqueueResponseDto> serviceResult = new();
        try
        {
            if (String.IsNullOrEmpty(enqueueRequestDto.StorageUri)) 
                throw new Exception("Clip does not containe a Storage Uri.");
            if (String.IsNullOrEmpty(enqueueRequestDto.UserId)) 
                throw new Exception("Clip does not containe a UserId.");

            var queueMessageDto = new QueueMessageDto()
            {
                Name = enqueueRequestDto.Name,
                Description = enqueueRequestDto.Description,
                StorageUri = enqueueRequestDto.StorageUri,
                ClipUserId = enqueueRequestDto.UserId,
                ClipId = enqueueRequestDto.ClipId
            };
            var queueMessageAsJson = JsonConvert.SerializeObject(queueMessageDto);
            Azure.Response<SendReceipt> response = await _queueClient.SendMessageAsync(queueMessageAsJson);

            if (response.GetRawResponse().IsError) 
                throw new Exception(response.GetRawResponse().ReasonPhrase);

            serviceResult.Result = new EnqueueResponseDto()
            {
                Id = response.Value.MessageId
            };

        }
        catch (Exception ex)
        {
            serviceResult.IsError = true;
            serviceResult.ErrorMessage = ex.Message;
        }

        return serviceResult;
    }
}
