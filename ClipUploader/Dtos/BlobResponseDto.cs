﻿using ClipUploader.Models;

namespace ClipUploader.Dtos;

public class BlobResponseDto
{
    public Clip Clip { get; set; }
    public string? ErrorMessage {  get; set; }

    public BlobResponseDto()
    {
        Clip = new Clip();
    }
}
