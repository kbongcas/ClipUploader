﻿namespace ClipUploader.Models;

public class Clip
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Uri { get; set; }
    public IFormFile? File { get; set; }
}
