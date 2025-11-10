using ElectricVehicleDealer.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IReportService _reportService;

        public DashboardController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Tổng quan (tổng doanh thu, khách hàng, đơn hàng)
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalRevenue = await _reportService.GetTotalRevenueAsync();
            var totalCustomers = await _reportService.GetTotalCustomersAsync();
            var totalOrders = await _reportService.GetTotalOrdersAsync();

            var result = new
            {
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalOrders = totalOrders
            };

            return Ok(result);
        }

        /// <summary>
        /// Doanh thu theo tháng (dùng cho biểu đồ line/bar)
        /// </summary>
        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth()
        {
            var data = await _reportService.GetRevenueByMonthAsync();

            var result = data.Select(x => new
            {
                Month = x.Month,
                Revenue = x.Revenue
            });

            return Ok(result);
        }

        /// <summary>
        /// Top xe bán chạy nhất
        /// </summary>
        [HttpGet("top-vehicles")]
        public async Task<IActionResult> GetTopVehicles([FromQuery] int top = 5)
        {
            var data = await _reportService.GetTopVehiclesAsync(top);

            var result = data.Select(x => new
            {
                ModelName = x.ModelName,
                Quantity = x.Quantity
            });

            return Ok(result);
        }

        /// <summary>
        /// Tồn kho theo từng model xe
        /// </summary>
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory()
        {
            var data = await _reportService.GetInventoryAsync();

            var result = data.Select(x => new
            {
                ModelName = x.ModelName,
                Stock = x.Stock
            });

            return Ok(result);
        }
    }
}
