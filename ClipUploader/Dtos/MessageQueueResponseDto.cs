namespace ClipUploader.Dtos;

public class MessageQueueResponseDto
{
    public string Message {  get; set; }
    public string? Status {  get; set; }
    public string? ErrorMessage {  get; set; }
}
