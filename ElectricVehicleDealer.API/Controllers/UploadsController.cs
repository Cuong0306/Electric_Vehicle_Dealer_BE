using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

public class ImageUploadRequest
{
    public IFormFile File { get; set; } = default!;
}

public class MultiImageUploadRequest
{
    public List<IFormFile> Files { get; set; } = new();
}

[ApiController]
[Route("api/uploads")]
public class UploadsController : ControllerBase
{
    private readonly ICloudinaryService _cloud;

    public UploadsController(ICloudinaryService cloud) => _cloud = cloud;

    
    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest request, [FromQuery] string? folder, CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest("File is empty");

        var res = await _cloud.UploadImageAsync(request.File, folder, ct);
        return Ok(new
        {
            message = "Uploaded",
            publicId = res.PublicId,
            url = res.SecureUrl?.ToString(),
            width = res.Width,
            height = res.Height,
            format = res.Format,
            bytes = res.Bytes
        });
    }

    
    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UploadImages([FromForm] MultiImageUploadRequest request, [FromQuery] string? folder, CancellationToken ct)
    {
        if (request.Files is null || request.Files.Count == 0)
            return BadRequest("No files");

        var tasks = request.Files.Select(f => _cloud.UploadImageAsync(f, folder, ct));
        var results = await Task.WhenAll(tasks);

        return Ok(results.Select(res => new
        {
            publicId = res.PublicId,
            url = res.SecureUrl?.ToString(),
            width = res.Width,
            height = res.Height,
            format = res.Format,
            bytes = res.Bytes
        }));
    }
}
