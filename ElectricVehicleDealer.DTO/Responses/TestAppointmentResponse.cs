using ElectricVehicleDealer.DAL.Enum;
using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class TestAppointmentResponse
    {
        public int TestAppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TestAppointmentEnum Status { get; set; }
    }
}
