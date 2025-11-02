using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class AllocateVehicleDto
    {
        [Required(ErrorMessage = "VehicleId là bắt buộc.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Danh sách store không được để trống.")]
        public List<StoreAllocationInfo> Stores { get; set; } = new();
    }

    public class StoreAllocationInfo
    {
        [Required(ErrorMessage = "StoreId là bắt buộc.")]
        public int StoreId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; } = 1;
    }
}
