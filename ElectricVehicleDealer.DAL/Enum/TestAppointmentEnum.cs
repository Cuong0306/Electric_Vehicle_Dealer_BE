using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Enum
{
    public enum TestAppointmentEnum
    {
        Draft = 1,          // Bản nháp
        PendingSign = 2,    // Chờ ký
        Active = 3,         // Đang hiệu lực
        Expired = 4,        // Đã hết hạn
        Terminated = 5,     // Đã chấm dứt
        Cancelled = 6       // Đã hủy
    }
}
