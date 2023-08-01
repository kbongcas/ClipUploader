namespace ClipUploader.Dtos;

public class BlobResponseDto
{
    public BlobDto Blob { get; set; }
    public string? Status {  get; set; }
    public string? ErrorMessage {  get; set; }

    public BlobResponseDto()
    {
        Blob = new BlobDto();
    }
}
