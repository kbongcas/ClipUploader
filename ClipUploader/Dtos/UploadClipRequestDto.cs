using System.ComponentModel.DataAnnotations;

namespace ClipUploader.Dtos;

public class UploadClipRequestDto
{
    [Required, StringLength(60, MinimumLength = 1)]
    public string Name { get; set; } 

    [StringLength(60)]
    public string? Description { get; set; }

    public bool Public { get; set; }

    public IFormFile File { get; set; } 
}
