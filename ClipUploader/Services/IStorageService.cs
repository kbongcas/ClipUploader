﻿using ClipUploader.Dtos;
using ClipUploader.Models;

namespace ClipUploader.Services;

public interface IStorageService
{
    Task<BlobResponseDto> UploadAsync(Clip clip);
}