using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class GetAllTestAppointmentByStoreResponse
    {
        public int TestAppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public string DealerName { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public TestAppointmentEnum Status { get; set; }
    }
}
