using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateTestAppointmentRequest
    {
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
    }
}
