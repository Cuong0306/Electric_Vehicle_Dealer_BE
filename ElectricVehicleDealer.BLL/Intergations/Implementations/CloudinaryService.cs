using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql.BackendMessages;

public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken ct = default);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _config;

    public CloudinaryService(Cloudinary cloudinary, IConfiguration config)
    {
        _cloudinary = cloudinary;
        _config = config;
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(file.ContentType))
            throw new InvalidOperationException("Unsupported file type");

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder ?? _config["Cloudinary:Folder"],
            UseFilename = true,            
            UniqueFilename = true,         
            Overwrite = false,
         
        };

        var result = await _cloudinary.UploadAsync(uploadParams, ct);
        if (result.StatusCode is not System.Net.HttpStatusCode.OK)
            throw new InvalidOperationException($"Upload failed: {result.Error?.Message}");

        return result;
    }

   
}
