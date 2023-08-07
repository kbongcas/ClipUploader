using ClipUploader.Dtos;
using ClipUploader.Models;
using ClipUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClipDataService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClipsController : ControllerBase
{
    private IStorageService _storageService;
    private IQueueService _queueService;

    public ClipsController(
        IStorageService storageService,
        IQueueService queueService
        )
    {
        _storageService = storageService;
        _queueService = queueService;
    }

    [HttpPost]
    [RequestSizeLimit(200_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        //@TODO
        //  - Extract some of this logic into own service?
        //  - Validate content type
        // var contentType = file.ContentType;
        var id = Guid.NewGuid().ToString();
        var clip = new Clip() { Id = id, Name = file.FileName, File = file };

        var uploadResponse = await _storageService.UploadAsync(clip);
        clip = uploadResponse.Clip;
        if(uploadResponse.ErrorMessage != null) return StatusCode(StatusCodes.Status500InternalServerError);

        var queueResponse = await _queueService.Enqueue(clip);
        if(queueResponse.ErrorMessage != null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok();
    }

}
