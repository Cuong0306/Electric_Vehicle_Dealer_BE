using ElectricVehicleDealer.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        /// Tổng quan cơ bản (doanh thu, khách hàng, đơn hàng, tổng xe bán ra)
        /// </summary>
        [Authorize]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] int storeId)
        {
            var totalRevenue = await _reportService.GetTotalRevenueAsync(storeId);
            var totalCustomers = await _reportService.GetTotalCustomersAsync(storeId);
            var totalOrders = await _reportService.GetTotalOrdersAsync(storeId);
            var totalVehiclesSold = await _reportService.GetTotalVehiclesSoldAsync(storeId);

            return Ok(new
            {
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalOrders = totalOrders,
                TotalVehiclesSold = totalVehiclesSold
            });
        }

        /// <summary>
        /// Top dealer (thực hiện nhiều order nhất)
        /// </summary>
        [Authorize]
        [HttpGet("top-dealer")]
        public async Task<IActionResult> GetTopDealer([FromQuery] int storeId)
        {
            var topDealer = await _reportService.GetTopDealerAsync(storeId);
            return Ok(new { topDealer.DealerName, topDealer.OrdersCount });
        }

        /// <summary>
        /// Top customer (chi tiền nhiều nhất)
        /// </summary>
        [Authorize]
        [HttpGet("top-customer")]
        public async Task<IActionResult> GetTopCustomer([FromQuery] int storeId)
        {
            var topCustomer = await _reportService.GetTopCustomerAsync(storeId);
            return Ok(new { topCustomer.CustomerName, topCustomer.TotalSpent });
        }

        /// <summary>
        /// Doanh thu theo tháng (line/bar chart)
        /// </summary>
        [Authorize]
        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth([FromQuery] int storeId)
        {
            var revenueByMonth = await _reportService.GetRevenueByMonthAsync(storeId);
            return Ok(revenueByMonth.Select(x => new { x.Month, x.Revenue }));
        }

        /// <summary>
        /// Top 5 xe bán chạy nhất
        /// </summary>
        [Authorize]
        [HttpGet("top-vehicles")]
        public async Task<IActionResult> GetTopVehicles([FromQuery] int storeId, [FromQuery] int top = 5)
        {
            var topVehicles = await _reportService.GetTopVehiclesAsync(storeId, top);
            return Ok(topVehicles.Select(x => new { x.ModelName, x.Quantity }));
        }

        /// <summary>
        /// Top 5 xe ít được mua
        /// </summary>
        [Authorize]
        [HttpGet("bottom-vehicles")]
        public async Task<IActionResult> GetBottomVehicles([FromQuery] int storeId, [FromQuery] int top = 5)
        {
            var bottomVehicles = await _reportService.GetBottomVehiclesAsync(storeId, top);
            return Ok(bottomVehicles.Select(x => new { x.ModelName, x.Quantity }));
        }

        /// <summary>
        /// Tồn kho theo từng model xe
        /// </summary>
        [Authorize]
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory([FromQuery] int storeId)
        {
            var inventory = await _reportService.GetInventoryAsync(storeId);
            return Ok(inventory.Select(x => new { x.ModelName, x.Stock }));
        }
    }
}
