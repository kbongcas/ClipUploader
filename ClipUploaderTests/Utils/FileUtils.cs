using Microsoft.AspNetCore.Http;

namespace ClipUploaderTests.Utils;

public static class FileUtils
{
    public static IFormFile LoadFileFromDisk(string fileName)
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "Data/") + fileName;
        FormFile file = null;
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
        };
        return file;
    }
}
