using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ClipUploader.Dtos;
using ClipUploader.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Azure;

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

    public async Task<QueueMessageResponseDto> Enqueue(Clip clip)
    {
        QueueMessageResponseDto responseDto = new QueueMessageResponseDto();
        try
        {
            if (clip.Id == null) 
                throw new Exception("Clip does not container a Id.");

            Azure.Response<SendReceipt> response = await _queueClient.SendMessageAsync(clip.Id);

            if (response.GetRawResponse().IsError) 
                throw new Exception(response.GetRawResponse().ReasonPhrase);

            responseDto.Clip = clip;
        }
        catch (Exception ex)
        {
            // @TODO - Log error 
            responseDto.ErrorMessage = ex.Message;
        }

        return responseDto;
    }
}
