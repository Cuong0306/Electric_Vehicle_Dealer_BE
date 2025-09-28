using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class UpdateFeedbackRequest
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int VehicleId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
