using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public
        class AllocateVehicleDto
    {
        [Required(ErrorMessage = "Danh sách xe không được để trống.")]
        public List<int> VehicleIds { get; set; } = new();

        [Required(ErrorMessage = "StoreId là bắt buộc.")]
        public int StoreId { get; set; }
    }
}
