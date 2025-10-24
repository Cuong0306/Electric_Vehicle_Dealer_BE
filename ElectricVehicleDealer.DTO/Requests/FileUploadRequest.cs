using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; } = default!;
    }

    public class MultiFileUploadRequest
    {
        public List<IFormFile> Files { get; set; } = new();
    }

}
