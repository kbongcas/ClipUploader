using ClipUploader.Dtos;
using ClipUploader.Models;
using ClipUploader.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ClipDataService.Controllers;

[ApiController]
[Route("users/")]
public class ClipsController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IQueueService _queueService;
    private readonly IClipService _clipService;

    public ClipsController(
        IStorageService storageService,
        IQueueService queueService,
        IClipService clipService
        )
    {
        _storageService = storageService;
        _queueService = queueService;
        _clipService = clipService;
    }

    [HttpPost("my/clips")]
    [Authorize("AbleToWriteMyClips")]
    [RequestFormLimits(ValueCountLimit = 210_000_000, MultipartBodyLengthLimit = 210_000_000)]
    [RequestSizeLimit(210_000_000)]
    public async Task<IActionResult> UploadClip([FromForm]UploadClipRequestDto uploadClipRequestDto)
    {
        //@TODO
        //  - Validate content type - only accept mp4
        //  - Validate if Content exist
   
        // Add Clip
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var addClipRequestDto = new AddClipRequestDto()
        {
            UserId = userId,
            Name = uploadClipRequestDto.Name,
            Description = uploadClipRequestDto.Description,
            Public = uploadClipRequestDto.Public
        };
        var addClipResult = await _clipService.AddClipToUserAsync(addClipRequestDto);
        if(addClipResult.IsError)
        {
            Console.WriteLine("Error in Addeding clip");
            Console.WriteLine(addClipResult.ErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        // Upload Clip
        var storageUploadRequestDto = new StorageUploadRequestDto()
        {
            Id = addClipResult.Result.ClipId,
            File = uploadClipRequestDto.File
        };
        var uploadResult = await _storageService.UploadAsync(storageUploadRequestDto);
        if(uploadResult.IsError)
        {
            Console.WriteLine("Error in Uploading clip");
            Console.WriteLine(uploadResult.ErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // Add To queue
        var enqueueRequestDto = new EnqueueRequestDto()
        {
            Name = uploadClipRequestDto.Name,
            Description = uploadClipRequestDto.Description,
            UserId = userId,
            StorageUri = uploadResult.Result.StorageUri,
            ClipId = addClipResult.Result.ClipId
        };
        var enqueueResult = await _queueService.Enqueue(enqueueRequestDto);
        if (enqueueResult.IsError) {
            Console.WriteLine("Error in Enqueuing clip");
            Console.WriteLine(uploadResult.ErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        return Ok();
    }

    [HttpGet("healthcheck")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}
