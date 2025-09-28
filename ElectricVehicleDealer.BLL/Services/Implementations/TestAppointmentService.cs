using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class TestAppointmentService : ITestAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TestAppointmentService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<TestAppointmentResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<TestAppointment>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<TestAppointmentResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<TestAppointment>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<TestAppointmentResponse> CreateAsync(CreateTestAppointmentRequest dto)
        {
            var entity = new TestAppointment()
            {
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                DealerId = dto.DealerId,
                AppointmentDate = dto.AppointmentDate,
                Status = dto.Status,
            };
            await _unitOfWork.Repository<TestAppointment>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<TestAppointmentResponse> UpdateAsync(int id, UpdateTestAppointmentRequest dto)
        {
            var entity = await _unitOfWork.Repository<TestAppointment>().GetByIdAsync(id);
            entity.CustomerId = dto.CustomerId;
            entity.VehicleId = dto.VehicleId;
            entity.DealerId = dto.DealerId;
            entity.AppointmentDate = dto.AppointmentDate;
            entity.Status = dto.Status;
            _unitOfWork.Repository<TestAppointment>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<TestAppointment>().GetByIdAsync(id);
            _unitOfWork.Repository<TestAppointment>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static TestAppointmentResponse MapToResponse(TestAppointment x) => new TestAppointmentResponse
        {
            TestAppointmentId = x.TestAppointmentId,
            CustomerId = x.CustomerId,
            VehicleId = x.VehicleId,
            DealerId = x.DealerId,
            AppointmentDate = x.AppointmentDate,
            Status = x.Status,
        };
    }
}
