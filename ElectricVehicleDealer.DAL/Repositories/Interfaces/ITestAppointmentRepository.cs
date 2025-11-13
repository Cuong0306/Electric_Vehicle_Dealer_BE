using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface ITestAppointmentRepository : IGenericRepository<TestAppointment>
    {
        Task<List<GetAllTestAppointmentByStoreResponse>> GetAppointmentsByStoreIdAsync(int storeId);
    }
}
