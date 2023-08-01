using ClipUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClipDataService.Controllers;

[ApiController]
[Route("[controller]")]
public class ClipsController : ControllerBase
{
    private IStorageService _storageService;

    public ClipsController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [RequestSizeLimit(200_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var reponse = await _storageService.UploadAsync(file);
        if(reponse.ErrorMessage != null)
        {
            return Ok();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
