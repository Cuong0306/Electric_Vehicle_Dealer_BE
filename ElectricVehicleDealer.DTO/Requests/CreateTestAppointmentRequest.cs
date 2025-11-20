using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DTO.Config;
using System;
using System.Text.Json.Serialization;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateTestAppointmentRequest
    {
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TestAppointmentEnum? Status { get; set; }
    }
}
