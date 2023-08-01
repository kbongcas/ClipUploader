using ClipUploader.Models;

namespace ClipUploader.Dtos;

public class QueueMessageResponseDto
{

    public Clip Clip { get; set; }
    public string? ErrorMessage {  get; set; }

    public QueueMessageResponseDto()
    {
        Clip = new Clip();
    }
}
