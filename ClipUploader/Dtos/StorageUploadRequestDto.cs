namespace ClipUploader.Dtos;

public class StorageUploadRequestDto
{
    public string Id { get; set; }
    public IFormFile File { get; set; }
}
