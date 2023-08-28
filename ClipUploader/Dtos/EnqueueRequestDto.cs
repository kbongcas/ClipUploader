namespace ClipUploader.Dtos;

public class EnqueueRequestDto
{
    public string Name {  get; set; }
    public string Description {  get; set; }
    public string StorageUri { get; set; }
    public string UserId { get; set; }
    public string ClipId {  get; set; }
}
