using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class GetAllVehicleResponse
    {
        public int VehicleId { get; set; }

        public int? BrandId { get; set; }

        public string ModelName { get; set; } = null!;

        public string[] ImageUrls { get; set; } = Array.Empty<string>();

        public string? Version { get; set; }

        public int? Year { get; set; }

        public string? Color { get; set; }

        public decimal? Price { get; set; }

        public string? BatteryCapacity { get; set; }

        public string? RangePerCharge { get; set; }

        public string? WarrantyPeriod { get; set; }

        public int? SeatingCapacity { get; set; }           // 4 seats

        public string? Transmission { get; set; }           // Automatic

        public int? Airbags { get; set; }                   // 1 airbag

        public int? Horsepower { get; set; }                // 43 HP

        public string? VehicleType { get; set; }            // Minicar

        public int? TrunkCapacity { get; set; }             // 285 L

        public int? DailyDrivingLimit { get; set; }         // 300 km/day

        public string? Screen { get; set; }                 // màn hình (kích thước, OS, CarPlay/AA...)

        public string? SeatMaterial { get; set; }           // chất liệu ghế (da/nỉ/synthetic...)

        public string? InteriorMaterial { get; set; }       // chất liệu nội thất (nhựa mềm, ốp gỗ...)

        public string? AirConditioning { get; set; }        // Điều hòa (1 vùng/2 vùng, tự động...)

        public string? SpeakerSystem { get; set; }          // Loa (số loa, thương hiệu)

        public string? InVehicleCabinet { get; set; }       // Tủ trên xe (có/loại/kiểu bố trí)

        public int? LengthMm { get; set; }                  // dài (mm)

        public int? WidthMm { get; set; }                   // rộng (mm)

        public int? HeightMm { get; set; }                  // cao (mm)

        public string? Wheels { get; set; }                 // bánh xe (mâm, kích thước lốp)

        public string? Headlights { get; set; }             // đèn trước (Halogen/LED/Matrix...)

        public string? Taillights { get; set; }             // đèn sau (LED, dạng đồ họa...)

        public string? FrameChassis { get; set; }           // khung xe (monocoque, body-on-frame...)

        public int? DoorCount { get; set; }                 // số lượng cửa

        public string? GlassWindows { get; set; }           // kính xe (cách nhiệt, tối màu...)

        public string? Mirrors { get; set; }                // gương (gập điện, sấy, cảnh báo điểm mù)

        public string? Cameras { get; set; }

        public bool IsAllocation { get; set; } = false;

        public DateTime? CreateDate { get; set; }
    }
}
