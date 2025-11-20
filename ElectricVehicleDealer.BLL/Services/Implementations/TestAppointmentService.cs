using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class TestAppointmentService : ITestAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public TestAppointmentService(IUnitOfWork uow, IEmailService emailService)
        {
            _unitOfWork = uow;
            _emailService = emailService;
        }

        public async Task<IEnumerable<TestAppointmentResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<TestAppointment>().GetAllAsync();

            // Lưu ý: Nếu muốn check Expired hàng loạt khi lấy danh sách thì bỏ comment đoạn dưới
            /*
            foreach (var item in list)
            {
                // SỬA LOGIC: Không check nếu đã Completed hoặc Rejected
                if (item.Status != TestAppointmentEnum.Completed && 
                    item.Status != TestAppointmentEnum.Rejected && 
                    item.AppointmentDate < DateTime.Now && 
                    item.Status != TestAppointmentEnum.Expired)
                {
                    item.Status = TestAppointmentEnum.Expired;
                    _unitOfWork.Repository<TestAppointment>().Update(item);
                }
            }
            await _unitOfWork.SaveAsync();
            */

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
                Status = dto.Status ?? TestAppointmentEnum.Pending,
            };

            //Kiểm tra quá hạn ---
            CheckAndSetExpiry(entity);

            await _unitOfWork.Repository<TestAppointment>().AddAsync(entity);
            await _unitOfWork.SaveAsync();

            // Chỉ gửi mail nếu trạng thái cuối cùng vẫn là Accepted/Rejected
            if (entity.Status == TestAppointmentEnum.Accepted)
            {
                await SendAppointmentAcceptedEmailAsync(entity);
            }
            else if (entity.Status == TestAppointmentEnum.Rejected)
            {
                await SendAppointmentRejectedEmailAsync(entity);
            }

            return MapToResponse(entity);
        }

        public async Task<TestAppointmentResponse> UpdateAsync(int id, UpdateTestAppointmentRequest dto)
        {
            var entity = await _unitOfWork.Repository<TestAppointment>().GetByIdAsync(id);
            if (entity == null) throw new Exception($"Test Appointment {id} not found");

            // Lưu trạng thái cũ để so sánh gửi mail
            var oldStatus = entity.Status;

            // 1. Cập nhật ID (Chỉ update nếu có giá trị)
            if (dto.CustomerId.HasValue && dto.CustomerId.Value != 0)
                entity.CustomerId = dto.CustomerId.Value;

            if (dto.VehicleId.HasValue && dto.VehicleId.Value != 0)
                entity.VehicleId = dto.VehicleId.Value;

            if (dto.DealerId.HasValue && dto.DealerId.Value != 0)
                entity.DealerId = dto.DealerId.Value;

            // 2. Cập nhật Ngày hẹn
            if (dto.AppointmentDate.HasValue && dto.AppointmentDate.Value != default(DateTime))
                entity.AppointmentDate = dto.AppointmentDate.Value;

            // 3. Cập nhật Trạng thái từ DTO
            if (dto.Status.HasValue)
            {
                entity.Status = dto.Status.Value;
            }

            //Kiểm tra quá hạn
            CheckAndSetExpiry(entity);
            
            _unitOfWork.Repository<TestAppointment>().Update(entity);
            await _unitOfWork.SaveAsync();

            if (entity.Status != oldStatus)
            {
                if (entity.Status == TestAppointmentEnum.Accepted)
                {
                    await SendAppointmentAcceptedEmailAsync(entity);
                }
                else if (entity.Status == TestAppointmentEnum.Rejected)
                {
                    await SendAppointmentRejectedEmailAsync(entity);
                }
            }

            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<TestAppointment>().GetByIdAsync(id);
            if (entity == null) throw new Exception($"Test Appointment {id} not found");

            _unitOfWork.Repository<TestAppointment>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        //HÀM KIỂM TRA QUÁ HẠN
        private void CheckAndSetExpiry(TestAppointment entity)
        {
            // Nếu đã Completed HOẶC Rejected thì giữ nguyên trạng thái cũ
            if (entity.Status == TestAppointmentEnum.Completed || entity.Status == TestAppointmentEnum.Rejected)
            {
                return;
            }

            // Nếu thời gian hẹn nhỏ hơn thời gian hiện tại -> Set Expired
            if (entity.AppointmentDate < DateTime.Now)
            {
                entity.Status = TestAppointmentEnum.Expired;
            }
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

        public async Task<List<GetAllTestAppointmentByStoreResponse>> GetAppointmentsByStoreIdAsync(int storeId)
        {
            return await _unitOfWork.TestAppointments.GetAppointmentsByStoreIdAsync(storeId);
        }

        // --- EMAIL ACCEPTED ---
        private async Task SendAppointmentAcceptedEmailAsync(TestAppointment appointment)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(appointment.CustomerId);
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(appointment.VehicleId);
                var dealer = await _unitOfWork.Dealers.GetByIdAsync(appointment.DealerId);

                if (customer == null || string.IsNullOrEmpty(customer.Email)) return;

                string vehicleName = vehicle?.ModelName ?? "Xe";
                string dealerName = dealer?.FullName ?? "Đại lý";
                string dealerAddress = dealer?.Address ?? "Đang cập nhật";
                string appointmentTime = appointment.AppointmentDate.ToString("HH:mm 'ngày' dd/MM/yyyy");

                string subject = $"✅ Lịch lái thử #{appointment.TestAppointmentId} đã được DUYỆT!";
                string body = $@"
                    <h3>Xin chào {customer.FullName},</h3>
                    <p>Yêu cầu đặt lịch lái thử xe <b>{vehicleName}</b> của bạn đã được <b>CHẤP NHẬN</b>.</p>
                    
                    <div style='border: 1px solid #ccc; padding: 15px; border-radius: 5px; background-color: #f9f9f9;'>
                        <p><b>📋 Mã lịch:</b> {appointment.TestAppointmentId}</p>
                        <p><b>⏰ Thời gian:</b> {appointmentTime}</p>
                        <p><b>📍 Địa điểm:</b> {dealerName}</p>
                        <p><b>🏠 Địa chỉ:</b> {dealerAddress}</p>
                    </div>

                    <p>Vui lòng đến đúng giờ và mang theo giấy phép lái xe để nhân viên hỗ trợ bạn tốt nhất.</p>
                    <br/>
                    <p>Trân trọng,<br/><b>EV Dealer Team</b></p>
                ";

                await _emailService.SendEmailAsync(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send Accepted Email: {ex.Message}");
            }
        }

        // --- EMAIL REJECTED ---
        private async Task SendAppointmentRejectedEmailAsync(TestAppointment appointment)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(appointment.CustomerId);
                var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(appointment.VehicleId);
                var dealer = await _unitOfWork.Dealers.GetByIdAsync(appointment.DealerId);

                if (customer == null || string.IsNullOrEmpty(customer.Email)) return;

                string vehicleName = vehicle?.ModelName ?? "Xe";
                string dealerName = dealer?.FullName ?? "Đại lý";
                string appointmentTime = appointment.AppointmentDate.ToString("HH:mm 'ngày' dd/MM/yyyy");

                string subject = $"❌ Thông báo: Lịch lái thử #{appointment.TestAppointmentId} đã bị TỪ CHỐI";
                string body = $@"
                    <h3>Xin chào {customer.FullName},</h3>
                    <p>Chúng tôi rất tiếc phải thông báo rằng yêu cầu đặt lịch lái thử xe <b>{vehicleName}</b> của bạn vào lúc <b>{appointmentTime}</b> tại <b>{dealerName}</b> đã bị <b>TỪ CHỐI</b>.</p>
                    
                    <div style='border: 1px solid #ffcccc; padding: 15px; border-radius: 5px; background-color: #fff5f5;'>
                        <p><b>⚠️ Lý do:</b> Lịch trình của đại lý đã kín hoặc xe không sẵn sàng vào thời điểm này.</p>
                    </div>

                    <p>Quý khách vui lòng đặt lại lịch vào một thời điểm khác hoặc liên hệ trực tiếp với chúng tôi để được hỗ trợ.</p>
                    <br/>
                    <p>Trân trọng,<br/><b>EV Dealer Team</b></p>
                ";

                await _emailService.SendEmailAsync(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send Rejected Email: {ex.Message}");
            }
        }
    }
}