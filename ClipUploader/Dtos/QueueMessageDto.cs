﻿namespace ClipUploader.Dtos;

public class QueueMessageDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClipUserId {  get ; set; }
    public string StorageUri {  get ; set; }
    public string ClipId {  get ; set; }
}
