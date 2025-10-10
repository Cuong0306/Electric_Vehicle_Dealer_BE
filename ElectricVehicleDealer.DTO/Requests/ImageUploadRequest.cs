using Microsoft.AspNetCore.Http;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class ImageUploadRequest
    {
        public IFormFile File { get; set; } = default!;


    }


}
