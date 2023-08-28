namespace ClipUploader.Dtos;

public class UploadClipRequestDto
{
    public string Name { get; set; } 
    public string Description { get; set; }
    public IFormFile File { get; set; } 
}
