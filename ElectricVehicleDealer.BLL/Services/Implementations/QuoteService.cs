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
    public class QuoteService : IQuoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public QuoteService(IUnitOfWork uow, IEmailService emailService)
        {
            _unitOfWork = uow;
            _emailService = emailService;
        }

        // Lấy tất cả quote
        public async Task<IEnumerable<QuoteResponse>> GetAllAsync()
        {
            var quotes = await _unitOfWork.Repository<Quote>().GetAllAsync();
            var result = new List<QuoteResponse>();
            foreach (var quote in quotes)
            {
                result.Add(await MapToResponseAsync(quote));
            }
            return result;
        }

        // Lấy quote theo id
        public async Task<QuoteResponse> GetByIdAsync(int id)
        {
            var quote = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");
            return await MapToResponseAsync(quote);
        }

        // Tạo mới quote
        public async Task<QuoteResponse> CreateAsync(CreateQuoteRequest dto)
        {
            var quote = new Quote
            {
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                DealerId = dto.DealerId,
                TaxRate = dto.TaxRate ?? 0,
                PromotionId = dto.PromotionId,
                QuoteDate = dto.QuoteDate,
                Status = dto.Status ?? QuoteEnum.Draft
            };

            await _unitOfWork.Repository<Quote>().AddAsync(quote);
            await _unitOfWork.SaveAsync();

            // Gửi mail ngay nếu quote tạo xong là Accepted
            if (quote.Status == QuoteEnum.Accepted)
            {
                await SendQuoteAcceptedEmailAsync(quote);
            }

            return await MapToResponseAsync(quote);
        }

        // Cập nhật quote
        public async Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest dto)
        {
            var quote = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");

            var oldStatus = quote.Status;

            if (dto.CustomerId != 0) quote.CustomerId = dto.CustomerId;
            if (dto.VehicleId != 0) quote.VehicleId = dto.VehicleId;
            if (dto.DealerId != 0) quote.DealerId = dto.DealerId;
            if (dto.QuoteDate.HasValue) quote.QuoteDate = dto.QuoteDate.Value;
            if (dto.TaxRate.HasValue) quote.TaxRate = dto.TaxRate.Value;
            if (!string.IsNullOrEmpty(dto.Status)) quote.Status = ParseQuoteStatus(dto.Status);
            if (dto.PromotionId != 0) quote.PromotionId = dto.PromotionId;
            _unitOfWork.Repository<Quote>().Update(quote);
            await _unitOfWork.SaveAsync();

            // Gửi mail nếu status đổi thành Accepted
            if (quote.Status == QuoteEnum.Accepted && oldStatus != QuoteEnum.Accepted)
            {
                await SendQuoteAcceptedEmailAsync(quote);
            }

            return await MapToResponseAsync(quote);
        }

        // Xóa quote
        public async Task<bool> DeleteAsync(int id)
        {
            var quote = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");

            _unitOfWork.Repository<Quote>().Remove(quote);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // Parse status linh hoạt
        private static QuoteEnum ParseQuoteStatus(string status)
        {
            if (int.TryParse(status, out var numericStatus))
            {
                if (Enum.IsDefined(typeof(QuoteEnum), numericStatus))
                    return (QuoteEnum)numericStatus;
                throw new ArgumentException($"Invalid numeric quote status: {status}");
            }

            if (Enum.TryParse<QuoteEnum>(status, true, out var parsedEnum))
                return parsedEnum;

            throw new ArgumentException($"Invalid quote status: {status}");
        }

        // Map Quote -> QuoteResponse
        private async Task<QuoteResponse> MapToResponseAsync(Quote quote)
        {
            var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(quote.VehicleId);

            return new QuoteResponse
            {
                QuoteId = quote.QuoteId,
                CustomerId = quote.CustomerId,
                VehicleId = quote.VehicleId,
                DealerId = quote.DealerId,
                TaxRate = quote.TaxRate,
                QuoteDate = quote.QuoteDate,
                PromotionId = quote.PromotionId,
                Status = quote.Status,
                VehiclePrice = vehicle?.Price ?? 0m,
                PriceWithTax = (vehicle?.Price ?? 0m) + ((vehicle?.Price ?? 0m) * quote.TaxRate / 100)
            };
        }

        // Gửi mail thông báo quote Accepted
        private async Task SendQuoteAcceptedEmailAsync(Quote quote)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(quote.CustomerId);
            var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(quote.VehicleId);

            if (customer == null || string.IsNullOrEmpty(customer.Email)) return;
            if (vehicle == null) return;

            decimal basePrice = vehicle.Price ?? 0;
            decimal taxAmount = basePrice * quote.TaxRate / 100;
            decimal totalPrice = basePrice + taxAmount;

            string subject = $"Your Quote #{quote.QuoteId} has been Accepted!";

            string body = $@"
        <p>Hello <b>{customer.FullName}</b>,</p>

        <p>We’re excited to inform you that your quote <b>#{quote.QuoteId}</b> has been <b>accepted</b>.</p>

        <h3>🚗 Vehicle Details</h3>
        <ul>
            <li><b>Brand:</b> {vehicle.Brand}</li>
            <li><b>Model:</b> {vehicle.ModelName}</li>
            <li><b>Year:</b> {vehicle.Year}</li>
            <li><b>Base Price:</b> {basePrice:C}</li>
            <li><b>Tax:</b> {quote.TaxRate}% ({taxAmount:C})</li>
            <li><b>Total Price:</b> {totalPrice:C}</li>
        </ul>

        <p>Thank you for choosing our dealership!</p>
        <p>— EV Dealer Team</p>";

            await _emailService.SendEmailAsync(customer.Email, subject, body);
        }

    }
}
